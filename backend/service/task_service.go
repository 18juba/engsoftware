package service

import (
	"backend/model"
	"backend/repository"
)

type TaskService struct {
	repository repository.TaskRepository
}

func NewTaskService(repo repository.TaskRepository) TaskService {
	return TaskService{
		repository: repo,
	}
}

func (service *TaskService) Index(page, limit int) (model.PaginatedResponse[model.Task], error) {
	if page < 1 {
		page = 1
	}
	if limit < 1 || limit > 100 {
		limit = 20
	}

	tasks, total, err := service.repository.Index(page, limit)
	if err != nil {
		return model.PaginatedResponse[model.Task]{}, err
	}

	totalPages := total / int64(limit)
	if total%int64(limit) > 0 {
		totalPages++
	}

	return model.PaginatedResponse[model.Task]{
		Data:       tasks,
		Total:      total,
		Page:       page,
		Limit:      limit,
		TotalPages: totalPages,
	}, nil
}

func (service *TaskService) IndexByUser(userID, page, limit int) (model.PaginatedResponse[model.Task], error) {
	if page < 1 {
		page = 1
	}
	if limit < 1 || limit > 100 {
		limit = 20
	}

	tasks, total, err := service.repository.IndexByUser(userID, page, limit)
	if err != nil {
		return model.PaginatedResponse[model.Task]{}, err
	}

	totalPages := total / int64(limit)
	if total%int64(limit) > 0 {
		totalPages++
	}

	return model.PaginatedResponse[model.Task]{
		Data:       tasks,
		Total:      total,
		Page:       page,
		Limit:      limit,
		TotalPages: totalPages,
	}, nil
}

func (service *TaskService) Show(id string) (model.Task, error) {
	return service.repository.Show(id)
}

func (service *TaskService) Store(task model.Task) (model.Task, error) {
	return service.repository.Store(task)
}

func (service *TaskService) Update(id string, task model.Task) error {
	return service.repository.Update(id, task)
}

func (service *TaskService) UpdateWithMap(id string, updates map[string]interface{}) error {
	task := model.Task{}
	return service.repository.UpdateWithMap(id, task, updates)
}

func (service *TaskService) Delete(id string) error {
	return service.repository.Delete(id)
}

func (service *TaskService) Toggle(id string) (model.Task, error) {
	return service.repository.Toggle(id)
}
