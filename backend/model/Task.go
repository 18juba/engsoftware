package model

import "time"

type TaskStatus string
type TaskPriority string

const (
	TaskStatusScheduled  TaskStatus = "scheduled"
	TaskStatusInProgress TaskStatus = "in_progress"
	TaskStatusCancelled  TaskStatus = "cancelled"
	TaskStatusCompleted  TaskStatus = "completed"
	TaskStatusPaused     TaskStatus = "paused"

	TaskPriorityLow    TaskPriority = "low"
	TaskPriorityMedium TaskPriority = "medium"
	TaskPriorityHigh   TaskPriority = "high"
)

type Task struct {
	ID             int          `json:"id" gorm:"primaryKey"`
	UserID         *int         `json:"user_id" gorm:"not null;index"`
	Title          string       `json:"title" gorm:"not null"`
	Description    string       `json:"description" gorm:"type:text"`
	ScheduledTime  time.Time    `json:"scheduled_time" gorm:"not null"`
	CompletionTime *time.Time   `json:"completion_time"`
	Status         TaskStatus   `json:"status" gorm:"type:varchar(20);check:status IN ('scheduled','in_progress','cancelled','completed','paused');default:'scheduled'"`
	Priority       TaskPriority `json:"priority" gorm:"type:varchar(20);check:priority IN ('low','medium','high');default:'medium'"`
	CreatedAt      time.Time    `json:"created_at" gorm:"autoCreateTime"`

	User *User `gorm:"foreignKey:UserID" json:"user,omitempty"`
}

func (task *Task) OwnerID() *int { return task.UserID }
