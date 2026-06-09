package model

type Enrollment struct {
	ID        int         `json:"id" gorm:"primaryKey"`
	StudentID int         `json:"student_id" gorm:"not null"`
	Student   UserStudent `gorm:"foreignKey:StudentID" json:"student"`
	ClassID   int         `json:"class_id" gorm:"not null"`
	Class     Class       `gorm:"foreignKey:ClassID" json:"class"`
}
