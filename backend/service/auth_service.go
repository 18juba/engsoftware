package service

import (
	"backend/model"
	"backend/repository"
	"errors"
	"strconv"
	"strings"
	"time"

	"github.com/golang-jwt/jwt/v5"
	"golang.org/x/crypto/bcrypt"
)

var (
	ErrInvalidCredentials     = errors.New("credenciais inválidas")
	ErrEmailAlreadyRegistered = errors.New("email já cadastrado")
	ErrPasswordMismatch       = errors.New("confirmação de senha inválida")
)

type AuthService struct {
	repository repository.AuthRepository
	jwtSecret  []byte
	tokenTTL   time.Duration
}

func NewAuthService(repo repository.AuthRepository, jwtSecret string, tokenTTL time.Duration) AuthService {
	return AuthService{
		repository: repo,
		jwtSecret:  []byte(jwtSecret),
		tokenTTL:   tokenTTL,
	}
}

func (service *AuthService) Register(name string, email string, password string, passwordConfirmation string, whatsapp *string) (model.User, error) {
	if password != passwordConfirmation {
		return model.User{}, ErrPasswordMismatch
	}

	normalizedEmail := strings.ToLower(strings.TrimSpace(email))
	existingUser, err := service.repository.FindByEmail(normalizedEmail)
	if err != nil {
		return model.User{}, err
	}
	if existingUser != nil {
		return model.User{}, ErrEmailAlreadyRegistered
	}

	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(password), bcrypt.DefaultCost)
	if err != nil {
		return model.User{}, err
	}

	user := model.User{
		Name:     strings.TrimSpace(name),
		Email:    normalizedEmail,
		Password: string(hashedPassword),
		Active:   true,
		Whatsapp: whatsapp,
	}

	userID, err := service.repository.Store(user)
	if err != nil {
		return model.User{}, err
	}

	user.ID = userID
	user.Password = ""

	return user, nil
}

func (service *AuthService) Login(email string, password string) (string, time.Time, error) {
	normalizedEmail := strings.ToLower(strings.TrimSpace(email))
	user, err := service.repository.FindByEmail(normalizedEmail)
	if err != nil {
		return "", time.Time{}, err
	}
	if user == nil {
		return "", time.Time{}, ErrInvalidCredentials
	}

	if err := bcrypt.CompareHashAndPassword([]byte(user.Password), []byte(password)); err != nil {
		return "", time.Time{}, ErrInvalidCredentials
	}

	now := time.Now().UTC()
	expiresAt := now.Add(service.tokenTTL)
	claims := jwt.RegisteredClaims{
		Subject:   strconv.Itoa(user.ID),
		ExpiresAt: jwt.NewNumericDate(expiresAt),
		IssuedAt:  jwt.NewNumericDate(now),
		NotBefore: jwt.NewNumericDate(now),
		Issuer:    "camarao-api",
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	signedToken, err := token.SignedString(service.jwtSecret)
	if err != nil {
		return "", time.Time{}, err
	}

	return signedToken, expiresAt, nil
}
