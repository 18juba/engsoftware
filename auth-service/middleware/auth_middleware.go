package middleware

import (
	"errors"
	"net/http"
	"strconv"
	"strings"

	"backend/model"

	"github.com/gin-gonic/gin"
	"github.com/golang-jwt/jwt/v5"
)

const ContextUserIDKey = "authUserID"

func RequireAuth(jwtSecret string) gin.HandlerFunc {
	secret := strings.TrimSpace(jwtSecret)

	return func(context *gin.Context) {
		if secret == "" {
			response := model.Response{
				Code:    http.StatusInternalServerError,
				Message: "Configuração de autenticação inválida",
			}
			context.AbortWithStatusJSON(http.StatusInternalServerError, response)
			return
		}

		tokenString := extractToken(context)
		if tokenString == "" {
			response := model.Response{
				Code:    http.StatusUnauthorized,
				Message: "Token de autenticação ausente",
			}
			context.AbortWithStatusJSON(http.StatusUnauthorized, response)
			return
		}

		claims := &jwt.RegisteredClaims{}
		token, err := jwt.ParseWithClaims(tokenString, claims, func(token *jwt.Token) (interface{}, error) {
			if token.Method == nil || token.Method.Alg() != jwt.SigningMethodHS256.Alg() {
				return nil, errors.New("método de assinatura inválido")
			}
			return []byte(secret), nil
		})
		if err != nil || token == nil || !token.Valid {
			response := model.Response{
				Code:    http.StatusUnauthorized,
				Message: "Token inválido ou expirado",
			}
			context.AbortWithStatusJSON(http.StatusUnauthorized, response)
			return
		}

		userID, err := strconv.Atoi(claims.Subject)
		if err != nil || userID <= 0 {
			response := model.Response{
				Code:    http.StatusUnauthorized,
				Message: "Token inválido",
			}
			context.AbortWithStatusJSON(http.StatusUnauthorized, response)
			return
		}

		context.Set(ContextUserIDKey, userID)
		context.Next()
	}
}

func MustGetUserID(context *gin.Context) (int, bool) {
	value, exists := context.Get(ContextUserIDKey)
	if !exists {
		return 0, false
	}

	userID, ok := value.(int)
	if !ok || userID <= 0 {
		return 0, false
	}

	return userID, true
}

func extractToken(context *gin.Context) string {
	authHeader := strings.TrimSpace(context.GetHeader("Authorization"))
	if authHeader != "" {
		parts := strings.SplitN(authHeader, " ", 2)
		if len(parts) == 2 && strings.EqualFold(parts[0], "Bearer") {
			return strings.TrimSpace(parts[1])
		}
	}

	// Fallback para query param — usado apenas por streams SSE (EventSource)
	// que não suportam headers customizados nativamente
	if tokenParam := strings.TrimSpace(context.Query("token")); tokenParam != "" {
		return tokenParam
	}

	return ""
}
