package db

import (
	"database/sql"
	"fmt"
	"log"
	"os"

	"github.com/glebarez/sqlite"
	_ "github.com/tursodatabase/libsql-client-go/libsql"
	"gorm.io/gorm"
)

func ConnectDB(databaseURL string) (*gorm.DB, error) {
	dsn := databaseURL
	token := os.Getenv("DATABASE_TOKEN")
	if token != "" {
		dsn = fmt.Sprintf("%s?authToken=%s", databaseURL, token)
	}

	sqlDB, err := sql.Open("libsql", dsn)
	if err != nil {
		return nil, err
	}

	db, err := gorm.Open(&sqlite.Dialector{
		Conn: sqlDB,
	}, &gorm.Config{})
	if err != nil {
		return nil, err
	}

	log.Printf("Conectado ao banco com sucesso.")
	return db, nil
}
