package repository

import (
	"backend/model"

	"gorm.io/gorm"
)

type UserRepository struct {
	db *gorm.DB
}

type ToDoTaskAnalysis struct {
	Status string `json:"status"`
	Count  int64  `json:"count"`
}

type UserDashboardResponse struct {
	ToDoTasksTotal    int64              `json:"to_do_tasks_total"`
	ToDoTasksAnalysis []ToDoTaskAnalysis `json:"to_do_tasks_analysis"`
}

func NewUserRepository(db *gorm.DB) UserRepository {
	return UserRepository{
		db: db,
	}
}

func (repository *UserRepository) Index() ([]model.User, error) {
	var users []model.User
	result := repository.db.Find(&users)
	return users, result.Error
}

func (repository *UserRepository) Store(user model.User) (int, error) {
	result := repository.db.Create(&user)
	return user.ID, result.Error
}

func (repository *UserRepository) Show(id int) (*model.User, error) {
	var user model.User
	result := repository.db.First(&user, id)
	if result.Error != nil {
		if result.Error == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, result.Error
	}
	return &user, nil
}

func (repository *UserRepository) ChangeCharacter(id int, character model.UserCharacter) error {
	var user model.User
	result := repository.db.First(&user, id)
	if result.Error != nil {
		return result.Error
	}
	user.Character = character
	result = repository.db.Save(&user)
	return result.Error
}

func (repository *UserRepository) Dashboard(userID int) (UserDashboardResponse, error) {
	var analysis []ToDoTaskAnalysis

	result := repository.db.
		Model(&model.Task{}).
		Select("status, COUNT(*) as count").
		Where("user_id = ? AND status IN ?", userID, []string{
			"in_progress",
			"scheduled",
			"paused",
		}).
		Group("status").
		Scan(&analysis)

	if result.Error != nil {
		return UserDashboardResponse{}, result.Error
	}

	var total int64
	for _, item := range analysis {
		total += item.Count
	}

	return UserDashboardResponse{
		ToDoTasksTotal:    total,
		ToDoTasksAnalysis: analysis,
	}, nil
}
