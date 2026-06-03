package model

import "time"

type UserType string

const (
	Admin    UserType = "admin"
	Customer UserType = "customer"
)

type User struct {
	ID        int       `json:"id" gorm:"primaryKey"`
	Type      UserType  `json:"type" gorm:"type:varchar(20);check:type IN ('admin','customer');default:'customer'"`
	Name      string    `json:"name" gorm:"not null"`
	Email     string    `json:"email" gorm:"uniqueIndex"`
	Password  string    `json:"-" gorm:"not null"`
	Whatsapp  *string   `json:"whatsapp"`
	Active    bool      `json:"active" gorm:"default:true"`
	CreatedAt time.Time `json:"created_at" gorm:"autoCreateTime"`

	Tasks         []Task         `gorm:"foreignKey:UserID" json:"tasks,omitempty"`
	Notifications []Notification `gorm:"foreignKey:UserID" json:"notifications,omitempty"`
}
