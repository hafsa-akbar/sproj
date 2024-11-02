#!/bin/sh

[ -d "Data/Migrations" ] && rm -rf "Data/Migrations"

dotnet ef migrations add InitialMigration -o "Data/Migrations"
dotnet ef migrations script
