package model

import "time"

type UserType string
type UserCharacter string

const (
	Customer UserType = "student"
	Teacher  UserType = "teacher"
	Admin    UserType = "admin"
)

const (
	Anonymous         UserCharacter = "anonymous"
	Bender            UserCharacter = "bender"
	Creeper           UserCharacter = "creeper"
	Doge              UserCharacter = "doge"
	Flameprincess     UserCharacter = "flameprincess"
	Freddy            UserCharacter = "freddy"
	Hellokitty        UserCharacter = "hellokitty"
	Homersimpson      UserCharacter = "homersimpson"
	Ironman           UserCharacter = "ironman"
	Jake              UserCharacter = "jake"
	Jason             UserCharacter = "jason"
	Joker             UserCharacter = "joker"
	Luigi             UserCharacter = "luigi"
	Mario             UserCharacter = "mario"
	Melody            UserCharacter = "melody"
	Minecraft         UserCharacter = "minecraft"
	Monalisa          UserCharacter = "monalisa"
	Princessbubblegum UserCharacter = "princessbubblegum"
	Scream            UserCharacter = "scream"
	Troll             UserCharacter = "troll"
)

type User struct {
	ID        int           `json:"id" gorm:"primaryKey"`
	Type      UserType      `json:"type" gorm:"type:varchar(20);check:type IN ('teacher','student','admin');default:'student'"`
	Character UserCharacter `json:"character" gorm:"type:varchar(20);check:character IN ('anonymous', 'bender', 'creeper', 'doge', 'flameprincess', 'freddy', 'hellokitty', 'homersimpson', 'ironman', 'jake', 'jason', 'joker', 'luigi', 'mario', 'melody', 'minecraft', 'monalisa', 'princessbubblegum', 'scream', 'troll');default:'homersimpson'"`
	Name      string        `json:"name" gorm:"not null"`
	Email     string        `json:"email" gorm:"uniqueIndex"`
	Password  string        `json:"-" gorm:"not null"`
	Whatsapp  *string       `json:"whatsapp"`
	Active    bool          `json:"active" gorm:"default:true"`
	CreatedAt time.Time     `json:"created_at" gorm:"autoCreateTime"`
}
