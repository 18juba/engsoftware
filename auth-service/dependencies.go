package main

import (
	"backend/controller"
	"backend/repository"
	"backend/service"

	"os"
	"strconv"
	"time"

	"gorm.io/gorm"
)

type Dependencies struct {
	HealthcheckController controller.HealthcheckController
	AuthController        controller.AuthController
}

func setupDependencies(dbConnection *gorm.DB) *Dependencies {
	healthcheck_controller := controller.NewHealthcheckController(dbConnection)

	auth_repository := repository.NewAuthRepository(dbConnection)
	auth_service := service.NewAuthService(auth_repository, getJWTSecret(), getTokenTTL())
	auth_controller := controller.NewAuthController(auth_service)

	return &Dependencies{
		HealthcheckController: healthcheck_controller,
		AuthController:        auth_controller,
	}
}

func getJWTSecret() string {
	return os.Getenv("JWT_SECRET")
}

func getTokenTTL() time.Duration {
	tokenTTL := time.Hour
	if tokenTTLMinutes := os.Getenv("JWT_TTL_MINUTES"); tokenTTLMinutes != "" {
		ttlMinutes, _ := strconv.Atoi(tokenTTLMinutes)
		tokenTTL = time.Duration(ttlMinutes) * time.Minute
	}
	return tokenTTL
}
