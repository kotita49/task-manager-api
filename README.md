# Task Manager API

Backend API for Task Manager application built with ASP.NET Core.

## Features
- Create, read, update, and delete tasks
- Task validation: due date must be in the future, status must be valid
- RESTful API with endpoints:
  - GET /api/tasks
  - GET /api/tasks/{id}
  - POST /api/tasks
  - PUT /api/tasks/{id}/status
  - DELETE /api/tasks/{id}

## Technologies
- ASP.NET Core
- Entity Framework Core
- SQLite / SQL Server

## Getting Started

1. Clone the repo:
```bash
git clone https://github.com/<your-username>/task-manager-api.git

2. Build and run:
dotnet build
dotnet run

3. Swagger UI available at: https://localhost:5001/swagger

Notes

Ensure .NET 8 SDK is installed

Modify appsettings.json for your database configuration
