package repository

import (
	"backend/model"

	"gorm.io/gorm"
)

type UserRepository struct {
	db *gorm.DB
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
