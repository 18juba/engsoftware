package controller

import (
	"backend/middleware"
	"backend/model"
	"backend/service"
	"net/http"

	"github.com/gin-gonic/gin"
)

func mustGetUser(context *gin.Context, user_service service.UserService) (*model.User, int, bool) {
	userID, ok := middleware.MustGetUserID(context)
	if !ok {
		context.JSON(http.StatusUnauthorized, model.Response{Code: http.StatusUnauthorized, Message: "Usuário não autenticado"})
		return nil, 0, false
	}

	user, err := user_service.Show(userID)
	if err != nil || user == nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao carregar usuário"})
		return nil, 0, false
	}

	return user, userID, true
}
