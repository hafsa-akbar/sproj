#!/bin/sh

[ -d "Migrations" ] && rm -rf "Migrations"

dotnet ef migrations add InitialMigration
dotnet ef migrations script
