#!/bin/bash

# Create a .pgpass file to store the PostgreSQL credentials
echo "$DB_HOST:5432:$DB_NAME:$DB_USER:$DB_PASSWORD" > ~/.pgpass
chmod 600 ~/.pgpass
echo "db_name: $DB_NAME:$DB_USER:$DB_PASSWORD -  $DB_HOST:5432:"
# Wait for PostgreSQL to be ready
until pg_isready -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME"; do
  echo "Waiting for PostgreSQL..."
  sleep 2
done

# Initialize the database
echo "Initializing the database..."
psql -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME" -c "
CREATE TABLE IF NOT EXISTS "TodoItems" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "IsComplete" BOOLEAN NOT NULL
);
"

exec "$@"