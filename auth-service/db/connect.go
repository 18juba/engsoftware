package db

import (
	"fmt"
	"log"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

func ConnectDB(host, port, user, password, dbname string) (*gorm.DB, error) {
	connection_string := fmt.Sprintf(
		"host=%s port=%s user=%s password=%s dbname=%s sslmode=disable",
		host, port, user, password, dbname,
	)

	db, err := gorm.Open(postgres.Open(connection_string), &gorm.Config{})
	if err != nil {
		return nil, err
	}

	log.Printf("Connected to %s", dbname)
	return db, nil
}
