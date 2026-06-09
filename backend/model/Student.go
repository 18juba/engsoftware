package model

type UserStudent struct {
	ID       int    `json:"id" gorm:"primaryKey"`
	UserID   int    `json:"user_id" gorm:"uniqueIndex"`
	User     User   `gorm:"foreignKey:UserID" json:"user"`
	Registry string `json:"registry" gorm:"not null"`
}
