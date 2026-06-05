package controller

import (
	"backend/middleware"
	"backend/model"
	"backend/service"
	"errors"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/go-playground/validator/v10"
)

type RegisterInput struct {
	Name                 string  `json:"name" binding:"required"`
	Email                string  `json:"email" binding:"required,email,max=255"`
	Password             string  `json:"password" binding:"required,min=8,max=255"`
	PasswordConfirmation string  `json:"passwordConfirmation" binding:"required,min=8,max=255"`
	WhatsApp             *string `json:"whatsapp"`
}

type LoginInput struct {
	Email    string `json:"email" binding:"required,email,max=255"`
	Password string `json:"password" binding:"required,min=8,max=255"`
}

type AuthController struct {
	auth_service service.AuthService
}

func NewAuthController(service service.AuthService) AuthController {
	return AuthController{
		auth_service: service,
	}
}

func (controller *AuthController) Register(context *gin.Context) {
	var input RegisterInput
	if err := context.ShouldBindJSON(&input); err != nil {
		response := model.Response{
			Code:    http.StatusBadRequest,
			Message: validationMessage(err, "Erro ao validar dados de cadastro"),
		}
		context.JSON(http.StatusBadRequest, response)
		return
	}

	createdUser, err := controller.auth_service.Register(input.Name, input.Email, input.Password, input.PasswordConfirmation, input.WhatsApp)
	if err != nil {
		if err == service.ErrPasswordMismatch {
			response := model.Response{
				Code:    http.StatusBadRequest,
				Message: "Confirmação de senha inválida",
			}
			context.JSON(http.StatusBadRequest, response)
			return
		}
		if err == service.ErrEmailAlreadyRegistered {
			response := model.Response{
				Code:    http.StatusConflict,
				Message: "E-mail já cadastrado",
			}
			context.JSON(http.StatusConflict, response)
			return
		}

		response := model.Response{
			Code:    http.StatusInternalServerError,
			Message: "Erro ao cadastrar usuário",
		}
		context.JSON(http.StatusInternalServerError, response)
		return
	}

	context.JSON(http.StatusCreated, createdUser)
}

func (a *AuthController) Login(context *gin.Context) {
	var input LoginInput
	if err := context.ShouldBindJSON(&input); err != nil {
		response := model.Response{
			Code:    http.StatusBadRequest,
			Message: validationMessage(err, "Erro ao validar credenciais"),
		}
		context.JSON(http.StatusBadRequest, response)
		return
	}

	token, expiresAt, user, err := a.auth_service.Login(input.Email, input.Password)
	if err != nil {
		if err == service.ErrInvalidCredentials {
			response := model.Response{
				Code:    http.StatusUnauthorized,
				Message: "Credenciais inválidas",
			}
			context.JSON(http.StatusUnauthorized, response)
			return
		}

		response := model.Response{
			Code:    http.StatusInternalServerError,
			Message: "Erro ao autenticar usuário",
		}
		context.JSON(http.StatusInternalServerError, response)
		return
	}

	context.JSON(http.StatusOK, gin.H{
		"token":     token,
		"expiresAt": expiresAt,
		"user":      user,
	})
}

func (controller *AuthController) Logout(context *gin.Context) {
	response := model.Response{
		Code:    http.StatusOK,
		Message: "Logout realizado com sucesso",
	}
	context.JSON(http.StatusOK, response)
}

func (controller *AuthController) Session(context *gin.Context) {
	userID, ok := middleware.MustGetUserID(context)
	if !ok {
		response := model.Response{
			Code:    http.StatusUnauthorized,
			Message: "Usuário não autenticado",
		}
		context.JSON(http.StatusUnauthorized, response)
		return
	}

	context.JSON(http.StatusOK, gin.H{
		"authenticated": true,
		"userId":        userID,
	})
}

func validationMessage(err error, fallback string) string {
	var validationErrs validator.ValidationErrors
	if !errors.As(err, &validationErrs) || len(validationErrs) == 0 {
		return fallback
	}

	fieldErr := validationErrs[0]
	switch fieldErr.Field() {
	case "Name":
		if fieldErr.Tag() == "required" {
			return "Nome é obrigatório"
		}
	case "Email":
		if fieldErr.Tag() == "required" {
			return "E-mail é obrigatório"
		}
		if fieldErr.Tag() == "email" {
			return "E-mail inválido"
		}
	case "Password":
		if fieldErr.Tag() == "required" {
			return "Senha é obrigatória"
		}
		if fieldErr.Tag() == "min" {
			return "Senha deve ter no mínimo 8 caracteres"
		}
	case "PasswordConfirmation":
		if fieldErr.Tag() == "required" {
			return "Confirmação de senha é obrigatória"
		}
	}

	return fallback
}
