# TodoApp Backend

Simple todo management API built with .NET 10.

## Architecture

Built with Clean Architecture principles:
- **Domain Layer**: Core entities and business logic
- **Application Layer**: CQRS with MediatR, DTOs, validation with FluentValidation
- **Infrastructure Layer**: EF Core, SQLite database, repositories
- **API Layer**: Minimal APIs with endpoint groups

## Features

- User authentication with JWT tokens
- Create, update, delete todos
- Mark todos as complete/incomplete
- Set deadlines for todos
- Filter by completion status

## Tech Stack

- .NET 10
- Entity Framework Core
- SQLite
- MediatR for CQRS
- FluentValidation
- AutoMapper
- JWT Authentication

## Running the Project

### With Docker
```bash
docker-compose up --build -d
```

The API will be available at http://localhost:5000

### Local Development
```bash
cd backend
dotnet restore
dotnet run --project TodoApp.API
```

### Running Tests
```bash
dotnet test
```

## API Endpoints

### Authentication
- POST /api/auth/register - Register new user
- POST /api/auth/login - Login user

### Todos (requires authentication)
- GET /api/todos - Get all todos
- GET /api/todos/{id} - Get todo by id
- POST /api/todos - Create new todo
- PUT /api/todos/{id} - Update todo
- DELETE /api/todos/{id} - Delete todo

## Database

SQLite database is automatically created on first run. Migrations are applied at startup.
