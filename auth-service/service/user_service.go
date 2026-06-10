package service

import (
	"backend/model"
	"backend/repository"

	"golang.org/x/crypto/bcrypt"
)

type UserService struct {
	repository repository.UserRepository
}

func NewUserService(repo repository.UserRepository) UserService {
	return UserService{
		repository: repo,
	}
}

func (service *UserService) Index() ([]model.User, error) {
	return service.repository.Index()
}

func (service *UserService) Show(id int) (*model.User, error) {
	user, err := service.repository.Show(id)

	if err != nil {
		return nil, err
	}

	return user, nil
}

func (service *UserService) Store(user model.User) (model.User, error) {
	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(user.Password), bcrypt.DefaultCost)
	if err != nil {
		return model.User{}, err
	}
	user.Password = string(hashedPassword)

	user_id, err := service.repository.Store(user)

	if err != nil {
		return model.User{}, err
	}

	user.ID = user_id

	return user, nil
}

func (service *UserService) ChangeCharacter(id int, character model.UserCharacter) error {
	return service.repository.ChangeCharacter(id, character)
}
