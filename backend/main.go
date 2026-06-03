package main

import (
	"github.com/gin-gonic/gin"
)

func main() {
	server := gin.Default()

	setupRoutes(server)

	server.Run(":8000")
}
