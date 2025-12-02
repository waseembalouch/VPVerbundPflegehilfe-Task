# TodoApp

A full-stack todo management application with user authentication.

## Project Structure

- **backend/** - .NET 10 API with Clean Architecture
- **frontend/todo-app/** - React TypeScript frontend

## Quick Start

### Prerequisites
- Docker and Docker Compose

### Running the Application

1. Clone the repository
2. Run with Docker:
```bash
docker-compose up --build -d
```

3. Access the application:
   - Frontend: http://localhost
   - Backend API: http://localhost:5000

### Stopping the Application
```bash
docker-compose down
```

## Features

- User authentication (register/login)
- Create todos with descriptions and deadlines
- Mark todos as complete/incomplete
- Delete todos
- Filter todos by status
- Responsive UI

## Architecture

### Backend
- Clean Architecture with CQRS pattern
- Entity Framework Core with SQLite
- JWT authentication
- MediatR for command/query handling
- FluentValidation for input validation

### Frontend
- React with TypeScript
- Context API for auth state
- Custom hooks for data fetching
- Optimistic UI updates
- Nginx proxy for API requests

## Development

See individual README files in backend and frontend directories for local development setup.

## Testing

Backend tests:
```bash
cd backend
dotnet test
```

Frontend tests:
```bash
cd frontend/todo-app
npm test
```
