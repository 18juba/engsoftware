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
	UserController        controller.UserController
	TaskController        controller.TaskController
}

func setupDependencies(dbConnection *gorm.DB) *Dependencies {
	healthcheck_controller := controller.NewHealthcheckController(dbConnection)

	auth_repository := repository.NewAuthRepository(dbConnection)
	auth_service := service.NewAuthService(auth_repository, getJWTSecret(), getTokenTTL())
	auth_controller := controller.NewAuthController(auth_service)

	user_repository := repository.NewUserRepository(dbConnection)
	user_service := service.NewUserService(user_repository)
	user_controller := controller.NewUserController(user_service)

	task_repository := repository.NewTaskRepository(dbConnection)
	task_service := service.NewTaskService(task_repository)
	task_controller := controller.NewTaskController(task_service, user_service)

	return &Dependencies{
		HealthcheckController: healthcheck_controller,
		AuthController:        auth_controller,
		UserController:        user_controller,
		TaskController:        task_controller,
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
