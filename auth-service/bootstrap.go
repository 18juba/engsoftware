package main

import (
	"log"
	"os"
	"strconv"
	"strings"
)

func validateEnv() {
	requiredVars := map[string]string{
		"DATABASE_URL":   "URL de conexão do banco de dados",
		"DATABASE_TOKEN": "Token de autenticação do banco de dados (Turso)",
		"JWT_SECRET":     "Secret para JWT",
		"CORS_ORIGINS":   "Origens CORS permitidas",
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
