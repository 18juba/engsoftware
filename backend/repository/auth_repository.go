package repository

import (
	"backend/model"
	"strings"

	"gorm.io/gorm"
)

type AuthRepository struct {
	db *gorm.DB
}

func NewAuthRepository(db *gorm.DB) AuthRepository {
	return AuthRepository{
		db: db,
	}
}

func (repository *AuthRepository) FindByEmail(email string) (*model.User, error) {
	var user model.User
	result := repository.db.Where("LOWER(email) = ?", strings.ToLower(email)).First(&user)
	if result.Error != nil {
		if result.Error == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, result.Error
	}

	return &user, nil
}

func (repository *AuthRepository) Store(user model.User) (int, error) {
	result := repository.db.Create(&user)
	return user.ID, result.Error
}
