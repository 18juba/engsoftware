package model

type Class struct {
	ID        int         `json:"id" gorm:"primaryKey"`
	TeacherID int         `json:"teacher_id" gorm:"not null"`
	Teacher   UserTeacher `gorm:"foreignKey:TeacherID" json:"teacher"`
	Semester  string      `json:"semester" gorm:"not null"`
	SubjectID int         `json:"subject_id" gorm:"not null"`
	Subject   Subject     `gorm:"foreignKey:SubjectID" json:"subject"`
}
