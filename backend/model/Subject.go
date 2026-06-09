package model

type Subject struct {
	ID        int         `json:"id" gorm:"primaryKey"`
	TeacherID int         `json:"teacher_id" gorm:"not null"`
	Teacher   UserTeacher `gorm:"foreignKey:TeacherID" json:"teacher"`
	Name      string      `json:"name" gorm:"not null"`
	Code      string      `json:"code" gorm:"uniqueIndex;not null"`
	Workload  int         `json:"workload" gorm:"not null"`
}
