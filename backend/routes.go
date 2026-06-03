package main

import (
	"github.com/gin-gonic/gin"
)

func setupRoutes(server *gin.Engine) {
	server.GET("/", func(c *gin.Context) {
		c.JSON(200, gin.H{
			"message": "Hello, World!",
		})
	})
}
