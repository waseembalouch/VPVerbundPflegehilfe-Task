# TodoApp Frontend

React-based frontend for the todo management application.

## Features

- User registration and login
- Create todos with descriptions and deadlines
- Mark todos as complete/incomplete
- Delete todos
- Filter todos by status (All/Active/Completed)
- Optimistic UI updates for better user experience
- Responsive design

## Tech Stack

- React 18
- TypeScript
- Vite
- React Router
- Context API for state management
- Vitest for testing

## Running the Project

### With Docker
```bash
docker-compose up --build -d
```

The app will be available at http://localhost

### Local Development
```bash
cd frontend/todo-app
npm install
npm run dev
```

The app will run on http://localhost:5173

### Running Tests
```bash
npm test
```

### Building for Production
```bash
npm run build
```

## Project Structure

```
src/
  components/     - React components
  context/        - Auth context
  hooks/          - Custom hooks (useTodos)
  services/       - API service layer
  types.ts        - TypeScript interfaces
```

## Configuration

Environment variables are in `.env.production`:
- VITE_API_URL - Backend API URL (default: /api)
- VITE_APP_NAME - Application name
- VITE_ENVIRONMENT - Environment (production/development)

## Authentication

The app uses JWT tokens stored in localStorage. Users must register/login to access todo features.
