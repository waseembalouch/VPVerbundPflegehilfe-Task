import { http, HttpResponse } from 'msw';
import type { AuthResponseDto, TodoDto, LoginRequest, RegisterRequest, CreateTodoRequest, UpdateTodoRequest } from '../../types';

const API_BASE_URL = 'http://localhost:5000/api';

// Mock data
export const mockUser: AuthResponseDto = {
  username: 'testuser',
  email: 'test@example.com',
  token: 'mock-jwt-token-12345',
};

export const mockTodos: TodoDto[] = [
  {
    id: 1,
    description: 'Complete project documentation for the new feature',
    isCompleted: false,
    deadline: '2025-12-31',
    createdAt: '2025-11-20T10:00:00Z',
    categoryId: 1,
    categoryName: 'Work',
    categoryColor: '#4ECDC4',
    tags: [{ id: 1, name: 'Urgent' }],
  },
  {
    id: 2,
    description: 'Review pull requests from team members',
    isCompleted: true,
    deadline: null,
    createdAt: '2025-11-21T10:00:00Z',
    categoryId: 1,
    categoryName: 'Work',
    categoryColor: '#4ECDC4',
    tags: [],
  },
  {
    id: 3,
    description: 'Buy groceries for weekly meal prep',
    isCompleted: false,
    deadline: '2025-11-30',
    createdAt: '2025-11-22T10:00:00Z',
    categoryId: 3,
    categoryName: 'Shopping',
    categoryColor: '#45B7D1',
    tags: [{ id: 3, name: 'Low Priority' }],
  },
];

// MSW Handlers
export const handlers = [
  // Auth - Login
  http.post(`${API_BASE_URL}/auth/login`, async ({ request }) => {
    const credentials = await request.json() as LoginRequest;

    if (credentials.username === 'testuser' && credentials.password === 'password123') {
      return HttpResponse.json(mockUser);
    }

    return HttpResponse.json(
      { message: 'Invalid username or password' },
      { status: 401 }
    );
  }),

  // Auth - Register
  http.post(`${API_BASE_URL}/auth/register`, async ({ request }) => {
    const data = await request.json() as RegisterRequest;

    if (data.username === 'existinguser') {
      return HttpResponse.json(
        { message: 'Username already exists' },
        { status: 400 }
      );
    }

    return HttpResponse.json(mockUser, { status: 201 });
  }),

  // Todos - Get all
  http.get(`${API_BASE_URL}/todos`, ({ request }) => {
    const token = request.headers.get('Authorization');

    if (!token || !token.startsWith('Bearer ')) {
      return HttpResponse.json(
        { message: 'Unauthorized' },
        { status: 401 }
      );
    }

    return HttpResponse.json(mockTodos);
  }),

  // Todos - Get by ID
  http.get(`${API_BASE_URL}/todos/:id`, ({ params }) => {
    const id = Number(params.id);
    const todo = mockTodos.find(t => t.id === id);

    if (!todo) {
      return HttpResponse.json(
        { message: 'Todo not found' },
        { status: 404 }
      );
    }

    return HttpResponse.json(todo);
  }),

  // Todos - Create
  http.post(`${API_BASE_URL}/todos`, async ({ request }) => {
    const token = request.headers.get('Authorization');

    if (!token || !token.startsWith('Bearer ')) {
      return HttpResponse.json(
        { message: 'Unauthorized' },
        { status: 401 }
      );
    }

    const data = await request.json() as CreateTodoRequest;

    const newTodo: TodoDto = {
      id: mockTodos.length + 1,
      description: data.description,
      isCompleted: false,
      deadline: data.deadline || null,
      createdAt: new Date().toISOString(),
      categoryId: data.categoryId || null,
      categoryName: 'Work',
      categoryColor: '#4ECDC4',
      tags: [],
    };

    return HttpResponse.json(newTodo, { status: 201 });
  }),

  // Todos - Update
  http.put(`${API_BASE_URL}/todos/:id`, async ({ params, request }) => {
    const token = request.headers.get('Authorization');

    if (!token || !token.startsWith('Bearer ')) {
      return HttpResponse.json(
        { message: 'Unauthorized' },
        { status: 401 }
      );
    }

    const id = Number(params.id);
    const data = await request.json() as UpdateTodoRequest;
    const todo = mockTodos.find(t => t.id === id);

    if (!todo) {
      return HttpResponse.json(
        { message: 'Todo not found' },
        { status: 404 }
      );
    }

    return HttpResponse.json(null, { status: 204 });
  }),

  // Todos - Delete
  http.delete(`${API_BASE_URL}/todos/:id`, ({ params, request }) => {
    const token = request.headers.get('Authorization');

    if (!token || !token.startsWith('Bearer ')) {
      return HttpResponse.json(
        { message: 'Unauthorized' },
        { status: 401 }
      );
    }

    const id = Number(params.id);
    const todo = mockTodos.find(t => t.id === id);

    if (!todo) {
      return HttpResponse.json(
        { message: 'Todo not found' },
        { status: 404 }
      );
    }

    return HttpResponse.json(null, { status: 204 });
  }),
];
