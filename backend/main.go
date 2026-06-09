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
	if err := godotenv.Load(); err != nil {
		log.Fatal("Erro ao carregar arquivo .env")
	}

	validateEnv()

	dbConnection, err := db.ConnectDB(
		os.Getenv("DB_HOST"),
		os.Getenv("DB_PORT"),
		os.Getenv("DB_USER"),
		os.Getenv("DB_PASSWORD"),
		os.Getenv("DB_NAME"),
	)

	if err != nil {
		log.Fatal(err)
	}

	err = dbConnection.AutoMigrate(
		&model.User{},
		&model.UserTeacher{},
		&model.UserStudent{},
		&model.Subject{},
		&model.Class{},
		&model.Enrollment{},
		&model.Task{},
		&model.TaskSubmit{},
		&model.Notification{},
	)

	if err != nil {
		log.Fatal("Erro ao migrar banco de dados:", err)
	}

	jwtSecret := os.Getenv("JWT_SECRET")
	authMiddleware := middleware.RequireAuth(jwtSecret)
	dependencies := setupDependencies(dbConnection)

	server := gin.Default()
	allowedOrigins := parseOrigins(os.Getenv("CORS_ORIGINS"))
	server.Use(middleware.CORS(allowedOrigins))

	setupRoutes(server, dependencies, authMiddleware)

	srv := &http.Server{
		Addr:    "0.0.0.0:8000",
		Handler: server,
	}

	go func() {
		log.Printf("Iniciando servidor em %s", srv.Addr)
		if err := srv.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			log.Fatalf("Erro ao iniciar servidor: %v", err)
		}
	}()

	sigChan := make(chan os.Signal, 1)
	signal.Notify(sigChan, syscall.SIGINT, syscall.SIGTERM)
	<-sigChan

	log.Println("Encerrando servidor...")
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	if err := srv.Shutdown(ctx); err != nil {
		log.Printf("Erro ao encerrar servidor: %v", err)
	}

	log.Println("Servidor encerrado")
}
