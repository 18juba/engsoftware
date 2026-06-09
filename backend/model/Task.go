package model

import "time"

type TaskStatus string

const (
	TaskStatusScheduled  TaskStatus = "scheduled"
	TaskStatusInProgress TaskStatus = "in_progress"
	TaskStatusCancelled  TaskStatus = "cancelled"
	TaskStatusCompleted  TaskStatus = "completed"
	TaskStatusPaused     TaskStatus = "paused"
)

type Task struct {
	ID             int        `json:"id" gorm:"primaryKey"`
	ClassID        int        `json:"class_id" gorm:"not null;index"`
	Class          *Class     `gorm:"foreignKey:ClassID" json:"class,omitempty"`
	Title          string     `json:"title" gorm:"not null"`
	Description    string     `json:"description" gorm:"type:text"`
	ScheduledTime  time.Time  `json:"scheduled_time" gorm:"not null"`
	CompletionTime *time.Time `json:"completion_time"`
	Status         TaskStatus `json:"status" gorm:"type:varchar(20);check:status IN ('scheduled','in_progress','cancelled','completed','paused');default:'scheduled'"`
	CreatedAt      time.Time  `json:"created_at" gorm:"autoCreateTime"`

	User *User `gorm:"foreignKey:UserID" json:"user,omitempty"`
}
