package controller

import (
	"backend/middleware"
	"backend/model"
	"backend/service"
	"net/http"

	"github.com/gin-gonic/gin"
)

type ownable interface {
	OwnerID() *int
}

func checkAccess(context *gin.Context, user *model.User, resource ownable) (int, bool) {
	userID, ok := middleware.MustGetUserID(context)
	if !ok {
		context.JSON(http.StatusUnauthorized, model.Response{Code: http.StatusUnauthorized, Message: "Não autenticado"})
		return 0, false
	}

	if user.Type == model.Admin {
		return userID, true
	}

	ownerID := resource.OwnerID()
	if ownerID == nil || int(*ownerID) != userID {
		context.JSON(http.StatusForbidden, model.Response{Code: http.StatusForbidden, Message: "Acesso negado"})
		return 0, false
	}

	return userID, true
}

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
