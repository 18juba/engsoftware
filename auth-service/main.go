package main

import (
	"backend/db"
	"backend/middleware"
	"backend/model"
	"context"
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/joho/godotenv"
)

func main() {
	_ = godotenv.Load()

	dbConnection, err := db.ConnectDB(os.Getenv("DATABASE_URL"))
	if err != nil {
		log.Fatal("Erro ao conectar ao banco:", err)
	}

	if err := dbConnection.AutoMigrate(&model.User{}); err != nil {
		log.Fatal("Erro ao migrar banco de dados:", err)
	}

	jwtSecret := os.Getenv("JWT_SECRET")

	deps := setupDependencies(dbConnection)

	authMiddleware := middleware.RequireAuth(jwtSecret)

	server := gin.Default()
	
	allowedOrigins := []string{
		os.Getenv("FRONTEND_URL"),
		os.Getenv("BACKEND_URL"),
		"http://localhost:3000",
	}

	server.Use(middleware.CORS(allowedOrigins))
	
	server.Use(gin.Logger(), gin.Recovery())
	server.Use(middleware.PrometheusMiddleware())
	
	setupRoutes(
		server,
		deps,
		authMiddleware,
	)

	port := os.Getenv("PORT")
	if port == "" {
		port = "8000"
	}

	srv := &http.Server{
		Addr:    "0.0.0.0:" + port,
		Handler: server,
	}

	go func() {
		log.Printf("Iniciando servidor em %s", srv.Addr)

		if err := srv.ListenAndServe(); err != nil &&
			err != http.ErrServerClosed {
			log.Fatalf("Erro ao iniciar servidor: %v", err)
		}
	}()

	sigChan := make(chan os.Signal, 1)
	signal.Notify(sigChan, syscall.SIGINT, syscall.SIGTERM)

	<-sigChan

	log.Println("Encerrando servidor...")

	ctx, cancel := context.WithTimeout(
		context.Background(),
		5*time.Second,
	)
	defer cancel()

	if err := srv.Shutdown(ctx); err != nil {
		log.Printf("Erro ao encerrar servidor: %v", err)
	}

	log.Println("Servidor encerrado")
}
