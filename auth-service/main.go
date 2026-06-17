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
	// Carrega variáveis de ambiente
	_ = godotenv.Load()

	// Banco de dados
	dbConnection, err := db.ConnectDB(os.Getenv("DATABASE_URL"))
	if err != nil {
		log.Fatal("Erro ao conectar ao banco:", err)
	}

	// Migrações
	if err := dbConnection.AutoMigrate(&model.User{}); err != nil {
		log.Fatal("Erro ao migrar banco de dados:", err)
	}

	// JWT
	jwtSecret := os.Getenv("JWT_SECRET")

	// Dependências da aplicação
	deps := setupDependencies(dbConnection)

	// Middleware de autenticação
	authMiddleware := middleware.RequireAuth(jwtSecret)

	// Servidor Gin
	server := gin.Default()

	// Rotas
	setupRoutes(
		server,
		deps,
		authMiddleware,
	)

	srv := &http.Server{
		Addr:    "0.0.0.0:8000",
		Handler: server,
	}

	// Inicialização do servidor
	go func() {
		log.Printf("Iniciando servidor em %s", srv.Addr)

		if err := srv.ListenAndServe(); err != nil &&
			err != http.ErrServerClosed {
			log.Fatalf("Erro ao iniciar servidor: %v", err)
		}
	}()

	// Graceful Shutdown
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