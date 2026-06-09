package model

type TaskSubmit struct {
	ID        int          `json:"id" gorm:"primaryKey"`
	StudentID *int         `json:"student_id" gorm:"not null;index"`
	Student   *UserStudent `gorm:"foreignKey:StudentID" json:"student,omitempty"`
	TaskID    int          `json:"task_id" gorm:"not null;index"`
	Task      *Task        `gorm:"foreignKey:TaskID" json:"task,omitempty"`
	Answer    string       `json:"answer" gorm:"type:text"`
}
