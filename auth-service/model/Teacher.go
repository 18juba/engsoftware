package model

type UserTeacher struct {
	ID         int    `json:"id" gorm:"primaryKey"`
	UserID     int    `json:"user_id" gorm:"uniqueIndex"`
	User       User   `gorm:"foreignKey:UserID" json:"user"`
	Department string `json:"department" gorm:"not null"`
	Siape      string `json:"siape"`
}
