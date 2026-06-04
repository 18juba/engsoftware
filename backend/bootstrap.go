package main

import (
	"log"
	"os"
	"strconv"
	"strings"
)

func validateEnv() {
	requiredVars := map[string]string{
		"DB_HOST":      "Host do banco de dados",
		"DB_PORT":      "Porta do banco de dados",
		"DB_USER":      "Usuário do banco de dados",
		"DB_PASSWORD":  "Senha do banco de dados",
		"DB_NAME":      "Nome do banco de dados",
		"JWT_SECRET":   "Secret para JWT",
		"CORS_ORIGINS": "Origens CORS permitidas",
	}

	for envVar, description := range requiredVars {
		if value := strings.TrimSpace(os.Getenv(envVar)); value == "" {
			log.Fatalf("Env var '%s' é obrigatório (%s)", envVar, description)
		}
	}

	if ttlMinutes := strings.TrimSpace(os.Getenv("JWT_TTL_MINUTES")); ttlMinutes != "" {
		ttl, err := strconv.Atoi(ttlMinutes)
		if err != nil || ttl <= 0 {
			log.Fatal("Env var 'JWT_TTL_MINUTES' deve ser um inteiro positivo")
		}
	}
}

func parseOrigins(raw string) []string {
	parts := strings.Split(raw, ",")
	origins := make([]string, 0, len(parts))

	for _, part := range parts {
		origin := strings.TrimSpace(part)
		if origin != "" {
			origins = append(origins, origin)
		}
	}

	return origins
}
