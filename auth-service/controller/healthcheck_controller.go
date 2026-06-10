package controller

import (
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
	"gorm.io/gorm"
)

type HealthcheckController struct {
	DB        *gorm.DB
	StartTime time.Time
}

func NewHealthcheckController(db *gorm.DB) HealthcheckController {
	return HealthcheckController{
		DB:        db,
		StartTime: time.Now(),
	}
}

func (controller *HealthcheckController) Check(context *gin.Context) {
	dbStatus := "connected"
	httpStatus := http.StatusOK

	sqlDB, err := controller.DB.DB()
	if err != nil || sqlDB.Ping() != nil {
		dbStatus = "error"
		httpStatus = http.StatusServiceUnavailable
	}

	context.JSON(httpStatus, gin.H{
		"status":    map[bool]string{true: "ok", false: "not_ok"}[httpStatus == http.StatusOK],
		"service":   "auth-service",
		"timestamp": time.Now().Format(time.RFC3339),
		"uptime":    time.Since(controller.StartTime).String(),
		"checks": gin.H{
			"database": dbStatus,
		},
	})
}
