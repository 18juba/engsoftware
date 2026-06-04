package main

import (
	"github.com/gin-gonic/gin"
)

func setupRoutes(server *gin.Engine, deps *Dependencies, authMiddleware gin.HandlerFunc) {
	server.POST("/auth/register", deps.AuthController.Register)
	server.POST("/auth/login", deps.AuthController.Login)
	server.POST("/auth/logout", deps.AuthController.Logout)
	server.GET("/auth/session", authMiddleware, deps.AuthController.Session)

	server.GET("/tasks", authMiddleware, deps.TaskController.Index)
	server.GET("/tasks/:id", authMiddleware, deps.TaskController.Show)
	server.POST("/tasks", authMiddleware, deps.TaskController.Create)
	server.PUT("/tasks/:id", authMiddleware, deps.TaskController.Update)
	server.DELETE("/tasks/:id", authMiddleware, deps.TaskController.Delete)
	server.PATCH("/tasks/:id/toggle", authMiddleware, deps.TaskController.Toggle)
}
