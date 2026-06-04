package controller

import (
	"backend/middleware"
	"backend/model"
	"backend/service"
	"net/http"
	"strconv"

	"github.com/gin-gonic/gin"
)

type CreateTaskRequest struct {
	Title         string  `json:"title" binding:"required"`
	Description   string  `json:"description" binding:"required"`
	ScheduledTime *string `json:"scheduled_time"`
}

type TaskController struct {
	taskService service.TaskService
	userService service.UserService
}

func NewTaskController(task_service service.TaskService, user_service service.UserService) TaskController {
	return TaskController{
		taskService: task_service,
		userService: user_service,
	}
}

func (controller *TaskController) Index(context *gin.Context) {
	user, userID, ok := mustGetUser(context, controller.userService)
	if !ok {
		return
	}

	page, _ := strconv.Atoi(context.DefaultQuery("page", "1"))
	limit, _ := strconv.Atoi(context.DefaultQuery("limit", "20"))

	var (
		result model.PaginatedResponse[model.Task]
		err    error
	)

	if user.Type == model.Admin {
		result, err = controller.taskService.Index(page, limit)
	} else {
		result, err = controller.taskService.IndexByUser(userID, page, limit)
	}

	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao listar tarefas"})
		return
	}

	context.JSON(http.StatusOK, result)
}

func (controller *TaskController) Show(context *gin.Context) {
	user, _, ok := mustGetUser(context, controller.userService)
	if !ok {
		return
	}

	id := context.Param("id")
	task, err := controller.taskService.Show(id)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao exibir tarefa"})
		return
	}

	if _, ok := checkAccess(context, user, &task); !ok {
		return
	}

	context.JSON(http.StatusOK, task)
}

func (controller *TaskController) Create(context *gin.Context) {
	var request CreateTaskRequest
	if err := context.ShouldBindJSON(&request); err != nil {
		context.JSON(http.StatusBadRequest, model.Response{
			Code:    http.StatusBadRequest,
			Message: "Dados inválidos",
		})
		return
	}

	var task model.Task
	task.Title = request.Title
	task.Description = request.Description

	if uid, ok := middleware.MustGetUserID(context); ok {
		id := int(uid)
		task.UserID = &id
	}

	createdTask, err := controller.taskService.Store(task)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{
			Code:    http.StatusInternalServerError,
			Message: "Erro ao criar tarefa",
		})
		return
	}
	context.JSON(http.StatusCreated, createdTask)
}

func (controller *TaskController) Update(context *gin.Context) {
	user, _, ok := mustGetUser(context, controller.userService)
	if !ok {
		return
	}

	id := context.Param("id")
	task, err := controller.taskService.Show(id)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao buscar tarefa"})
		return
	}

	if _, ok := checkAccess(context, user, &task); !ok {
		return
	}

	var payload struct {
		Name        string `json:"name"`
		Description string `json:"description"`
		ImageID     *int64 `json:"image_id"`
	}
	if err := context.ShouldBindJSON(&payload); err != nil {
		context.JSON(http.StatusBadRequest, model.Response{Code: http.StatusBadRequest, Message: "Dados inválidos"})
		return
	}

	updates := map[string]interface{}{
		"name":        payload.Name,
		"description": payload.Description,
		"image_id":    payload.ImageID,
	}
	if err := controller.taskService.UpdateWithMap(id, updates); err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao atualizar tarefa"})
		return
	}

	updatedTask, err := controller.taskService.Show(id)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao buscar tarefa atualizada"})
		return
	}

	context.JSON(http.StatusOK, updatedTask)
}

func (controller *TaskController) Delete(context *gin.Context) {
	user, _, ok := mustGetUser(context, controller.userService)
	if !ok {
		return
	}

	id := context.Param("id")
	task, err := controller.taskService.Show(id)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao buscar tarefa"})
		return
	}

	if _, ok := checkAccess(context, user, &task); !ok {
		return
	}

	if err := controller.taskService.Delete(id); err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao excluir tarefa"})
		return
	}

	context.JSON(http.StatusOK, model.Response{Code: http.StatusOK, Message: "Tarefa excluída com sucesso"})
}

func (controller *TaskController) Toggle(context *gin.Context) {
	user, _, ok := mustGetUser(context, controller.userService)
	if !ok {
		return
	}

	id := context.Param("id")
	task, err := controller.taskService.Show(id)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao buscar tarefa"})
		return
	}

	if _, ok := checkAccess(context, user, &task); !ok {
		return
	}

	updatedTask, err := controller.taskService.Toggle(id)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao fazer toggle da tarefa"})
		return
	}

	context.JSON(http.StatusOK, updatedTask)
}

func (controller *TaskController) AssignImage(context *gin.Context) {
	user, _, ok := mustGetUser(context, controller.userService)
	if !ok {
		return
	}

	id := context.Param("id")
	task, err := controller.taskService.Show(id)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao buscar tarefa"})
		return
	}

	if _, ok := checkAccess(context, user, &task); !ok {
		return
	}

	var payload struct {
		ImageID *int64 `json:"image_id"`
	}
	if err := context.ShouldBindJSON(&payload); err != nil {
		context.JSON(http.StatusBadRequest, model.Response{Code: http.StatusBadRequest, Message: "Dados inválidos"})
		return
	}

	if err := controller.taskService.UpdateWithMap(id, map[string]interface{}{"image_id": payload.ImageID}); err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao atualizar tarefa"})
		return
	}

	updatedTask, err := controller.taskService.Show(id)
	if err != nil {
		context.JSON(http.StatusInternalServerError, model.Response{Code: http.StatusInternalServerError, Message: "Erro ao buscar tarefa atualizada"})
		return
	}

	context.JSON(http.StatusOK, updatedTask)
}
