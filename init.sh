#!/bin/bash
set -e

# Create the table if it does not exist
psql -h postgres -U user -d todo_db <<EOF
CREATE TABLE IF NOT EXISTS "TodoItems" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "IsComplete" BOOLEAN NOT NULL
);
EOF