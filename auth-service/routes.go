package main

import (
	"github.com/gin-gonic/gin"
)

func setupRoutes(server *gin.Engine, deps *Dependencies, authMiddleware gin.HandlerFunc) {
	server.GET("/healthcheck", deps.HealthcheckController.Check)
	server.GET("/metrics", deps.HealthcheckController.Metrics)

	server.POST("/auth/register", deps.AuthController.Register)
	server.POST("/auth/login", deps.AuthController.Login)
	server.POST("/auth/logout", deps.AuthController.Logout)
	server.GET("/auth/session", authMiddleware, deps.AuthController.Session)

	server.PATCH("/users/change_character", authMiddleware, deps.UserController.ChangeCharacter)
}
