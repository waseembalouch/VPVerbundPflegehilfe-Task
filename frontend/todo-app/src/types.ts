export interface TodoDto {
  id: number;
  description: string;
  isCompleted: boolean;
  deadline: string | null;
  createdAt: string;
}

export interface AuthResponseDto {
  username: string;
  email: string;
  token: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface CreateTodoRequest {
  description: string;
  deadline?: string | null;
}

export interface UpdateTodoRequest {
  description: string;
  isCompleted: boolean;
  deadline?: string | null;
}

export type FilterType = 'All' | 'Active' | 'Completed';
