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
	repository  repository.AuthRepository
	jwtSecret   []byte
	tokenTTL    time.Duration
	jwtIssuer   string
	jwtAudience string
}

func NewAuthService(repo repository.AuthRepository, jwtSecret string, tokenTTL time.Duration, jwtIssuer string, jwtAudience string) AuthService {
	return AuthService{
		repository:  repo,
		jwtSecret:   []byte(jwtSecret),
		tokenTTL:    tokenTTL,
		jwtIssuer:   jwtIssuer,
		jwtAudience: jwtAudience,
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

func (service *AuthService) Login(email string, password string) (string, time.Time, *model.User, error) {
	normalizedEmail := strings.ToLower(strings.TrimSpace(email))
	user, err := service.repository.FindByEmail(normalizedEmail)
	if err != nil {
		return "", time.Time{}, nil, err
	}
	if user == nil {
		return "", time.Time{}, nil, ErrInvalidCredentials
	}

	if err := bcrypt.CompareHashAndPassword([]byte(user.Password), []byte(password)); err != nil {
		return "", time.Time{}, nil, ErrInvalidCredentials
	}

	now := time.Now().UTC()
	expiresAt := now.Add(service.tokenTTL)

	// 🔥 MODIFICAÇÃO: Usando MapClaims para garantir que 'aud' seja uma string simples
	// e que todos os campos obrigatórios estejam presentes conforme esperado pelo seu front/C#.
	claims := jwt.MapClaims{
		"sub": strconv.Itoa(user.ID),
		"exp": expiresAt.Unix(),
		"iat": now.Unix(),
		"nbf": now.Unix(),
		"iss": service.jwtIssuer,
		"aud": service.jwtAudience,
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	signedToken, err := token.SignedString(service.jwtSecret)
	if err != nil {
		return "", time.Time{}, nil, err
	}

	return signedToken, expiresAt, user, nil
}
