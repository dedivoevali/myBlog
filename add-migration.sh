#!/bin/bash

# Check if the migration name argument is provided
if [ -z "$1" ]; then
  echo "Usage: $0 <migration_name>"
  exit 1
fi

# Assign the provided migration name to a variable
migration_name=$1

# Run the EF Core migrations command
dotnet ef migrations add "$migration_name" -s Web -p Infrastructure -o ../Infrastructure/EFCore/Migrations