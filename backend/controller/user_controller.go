package controller

import (
	"backend/model"
	"backend/service"
	"net/http"
	"strconv"

	"github.com/gin-gonic/gin"
)

type UserInput struct {
	Name     string `json:"name" binding:"required"`
	Email    string `json:"email" binding:"required"`
	Password string `json:"password" binding:"required"`
}

type UserController struct {
	userService service.UserService
}

func NewUserController(service service.UserService) UserController {
	return UserController{
		userService: service,
	}
}

func (controller *UserController) Index(context *gin.Context) {

	users, err := controller.userService.Index()

	if err != nil {
		response := model.Response{
			Code:    http.StatusInternalServerError,
			Message: "Erro ao listar usuários",
		}
		context.JSON(http.StatusInternalServerError, response)
		return
	}

	context.JSON(http.StatusOK, users)
}

func (controller *UserController) Store(context *gin.Context) {
	var userInput UserInput

	err := context.BindJSON(&userInput)

	if err != nil {
		response := model.Response{
			Code:    http.StatusBadRequest,
			Message: "Erro ao validar dados do usuário",
		}
		context.JSON(http.StatusBadRequest, response)
		return
	}

	user := model.User{
		Name:     userInput.Name,
		Email:    userInput.Email,
		Password: userInput.Password,
	}

	created, err := controller.userService.Store(user)

	if err != nil {
		response := model.Response{
			Code:    http.StatusInternalServerError,
			Message: "Erro ao criar usuário",
		}
		context.JSON(http.StatusInternalServerError, response)
		return
	}

	context.JSON(http.StatusCreated, created)
}

func (controller *UserController) Show(context *gin.Context) {
	id := context.Param("id")

	if id == "" {
		response := model.Response{
			Code:    http.StatusBadRequest,
			Message: "ID do usuário não pode ser nulo",
		}

		context.JSON(http.StatusBadRequest, response)
		return
	}

	user_id, err := strconv.Atoi(id)

	if err != nil {
		response := model.Response{
			Code:    http.StatusBadRequest,
			Message: "ID do usuário precisa ser um numero",
		}

		context.JSON(http.StatusBadRequest, response)
		return
	}

	user, err := controller.userService.Show(user_id)

	if err != nil {
		response := model.Response{
			Code:    http.StatusInternalServerError,
			Message: "Erro ao buscar usuário",
		}
		context.JSON(http.StatusInternalServerError, response)
		return
	}

	if user == nil {
		response := model.Response{
			Code:    http.StatusNotFound,
			Message: "Usuário não encontrado",
		}

		context.JSON(http.StatusNotFound, response)
		return
	}

	context.JSON(http.StatusOK, user)
}
