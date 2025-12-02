import type {
  TodoDto,
  AuthResponseDto,
  LoginRequest,
  RegisterRequest,
  CreateTodoRequest,
  UpdateTodoRequest,
} from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

export interface ApiError extends Error {
  status: number;
  message: string;
}

export function isApiError(error: unknown): error is ApiError {
  return error instanceof Error && 'status' in error;
}

function createApiError(status: number, message: string): ApiError {
  const error = new Error(message) as ApiError;
  error.status = status;
  return error;
}

function getHeaders(includeAuth = false): HeadersInit {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };

  if (includeAuth) {
    const token = localStorage.getItem('token');
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
  }

  return headers;
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Request failed' }));

    let errorMessage = error.message || error.title || 'Request failed';
    if (error.errors) {
      const firstErrorKey = Object.keys(error.errors)[0];
      if (firstErrorKey && error.errors[firstErrorKey]?.length > 0) {
        errorMessage = error.errors[firstErrorKey][0];
      }
    }

    throw createApiError(response.status, errorMessage);
  }

  if (response.status === 204) {
    return null as T;
  }

  return response.json();
}

async function login(credentials: LoginRequest): Promise<AuthResponseDto> {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: 'POST',
    headers: getHeaders(),
    body: JSON.stringify(credentials),
  });

  return handleResponse<AuthResponseDto>(response);
}

async function register(data: RegisterRequest): Promise<AuthResponseDto> {
  const response = await fetch(`${API_BASE_URL}/auth/register`, {
    method: 'POST',
    headers: getHeaders(),
    body: JSON.stringify(data),
  });

  return handleResponse<AuthResponseDto>(response);
}

async function getTodos(): Promise<TodoDto[]> {
  const response = await fetch(`${API_BASE_URL}/todos`, {
    headers: getHeaders(true),
  });

  return handleResponse<TodoDto[]>(response);
}

async function getTodoById(id: number): Promise<TodoDto> {
  const response = await fetch(`${API_BASE_URL}/todos/${id}`, {
    headers: getHeaders(true),
  });

  return handleResponse<TodoDto>(response);
}

async function createTodo(todo: CreateTodoRequest): Promise<TodoDto> {
  const response = await fetch(`${API_BASE_URL}/todos`, {
    method: 'POST',
    headers: getHeaders(true),
    body: JSON.stringify(todo),
  });

  return handleResponse<TodoDto>(response);
}

async function updateTodo(id: number, todo: UpdateTodoRequest): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/todos/${id}`, {
    method: 'PUT',
    headers: getHeaders(true),
    body: JSON.stringify(todo),
  });

  return handleResponse<void>(response);
}

async function deleteTodo(id: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/todos/${id}`, {
    method: 'DELETE',
    headers: getHeaders(true),
  });

  return handleResponse<void>(response);
}

export const apiService = {
  login,
  register,
  getTodos,
  getTodoById,
  createTodo,
  updateTodo,
  deleteTodo,
};

