package repository

import (
	"backend/model"
	"time"

	"gorm.io/gorm"
)

type TaskRepository struct {
	db *gorm.DB
}

func NewTaskRepository(db *gorm.DB) TaskRepository {
	return TaskRepository{
		db: db,
	}
}

func (repository *TaskRepository) Index(page, limit int) ([]model.Task, int64, error) {
	var tasks []model.Task
	var total int64

	repository.db.Model(&model.Task{}).Count(&total)

	result := repository.db.
		Limit(limit).
		Offset((page - 1) * limit).
		Find(&tasks)

	return tasks, total, result.Error
}

func (repository *TaskRepository) IndexByUser(userID, page, limit int) ([]model.Task, int64, error) {
	var tasks []model.Task
	var total int64

	repository.db.Model(&model.Task{}).Where("user_id = ?", userID).Count(&total)

	result := repository.db.
		Where("user_id = ?", userID).
		Order("id DESC").
		Limit(limit).
		Offset((page - 1) * limit).
		Find(&tasks)

	return tasks, total, result.Error
}

func (repository *TaskRepository) Show(id string) (model.Task, error) {
	var task model.Task
	result := repository.db.Where("id = ?", id).First(&task)
	return task, result.Error
}

func (repository *TaskRepository) Store(task model.Task) (model.Task, error) {
	result := repository.db.Create(&task)
	return task, result.Error
}

func (repository *TaskRepository) Update(id string, task model.Task) error {
	result := repository.db.Model(&model.Task{}).Where("id = ?", id).Updates(task)
	return result.Error
}

func (repository *TaskRepository) UpdateWithMap(id string, task model.Task, updates map[string]interface{}) error {
	result := repository.db.Model(&task).Where("id = ?", id).Updates(updates)
	return result.Error
}

func (repository *TaskRepository) Delete(id string) error {
	result := repository.db.Where("id = ?", id).Delete(&model.Task{})
	return result.Error
}

func (repository *TaskRepository) Toggle(id string) (model.Task, error) {
	var task model.Task
	result := repository.db.Model(&task).Where("id = ?", id).Update("active", gorm.Expr("NOT active"))
	if result.Error != nil {
		return task, result.Error
	}
	return repository.Show(id)
}

func (repository *TaskRepository) StartTask(id string) (model.Task, error) {
	var task model.Task
	result := repository.db.Model(&task).Where("id = ?", id).Update("status", model.TaskStatusInProgress)
	if result.Error != nil {
		return task, result.Error
	}
	return repository.Show(id)
}

func (repository *TaskRepository) MarkAsComplete(id string) (model.Task, error) {
	var task model.Task
	updates := map[string]interface{}{
		"status":          model.TaskStatusCompleted,
		"completion_time": time.Now(),
	}
	result := repository.db.Model(&task).Where("id = ?", id).Updates(updates)
	if result.Error != nil {
		return task, result.Error
	}
	return repository.Show(id)
}

func (repository *TaskRepository) MarkAsCancelled(id string) (model.Task, error) {
	var task model.Task
	result := repository.db.Model(&task).Where("id = ?", id).Update("status", model.TaskStatusCancelled)
	if result.Error != nil {
		return task, result.Error
	}
	return repository.Show(id)
}
