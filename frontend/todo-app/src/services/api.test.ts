import { describe, it, expect, beforeEach } from 'vitest';
import { apiService } from './api';

describe('API Service', () => {
  beforeEach(() => {
    // Clear localStorage before each test
    localStorage.clear();
  });

  describe('Authentication', () => {
    it('successfully logs in with valid credentials', async () => {
      const result = await apiService.login({
        username: 'testuser',
        password: 'password123',
      });

      expect(result).toHaveProperty('username', 'testuser');
      expect(result).toHaveProperty('email', 'test@example.com');
      expect(result).toHaveProperty('token');
      expect(result.token).toBe('mock-jwt-token-12345');
    });

    it('throws error with invalid credentials', async () => {
      await expect(
        apiService.login({
          username: 'wronguser',
          password: 'wrongpass',
        })
      ).rejects.toThrow();
    });

    it('successfully registers new user', async () => {
      const result = await apiService.register({
        username: 'newuser',
        email: 'new@example.com',
        password: 'password123',
      });

      expect(result).toHaveProperty('username');
      expect(result).toHaveProperty('email');
      expect(result).toHaveProperty('token');
    });

    it('throws error when username already exists', async () => {
      await expect(
        apiService.register({
          username: 'existinguser',
          email: 'test@example.com',
          password: 'password123',
        })
      ).rejects.toThrow();
    });
  });

  describe('Todos', () => {
    beforeEach(() => {
      // Set up auth token
      localStorage.setItem('token', 'mock-jwt-token-12345');
    });

    it('fetches all todos', async () => {
      const todos = await apiService.getTodos();

      expect(todos).toBeInstanceOf(Array);
      expect(todos.length).toBeGreaterThan(0);
      expect(todos[0]).toHaveProperty('id');
      expect(todos[0]).toHaveProperty('description');
      expect(todos[0]).toHaveProperty('isCompleted');
    });

    it('throws error when fetching todos without authentication', async () => {
      localStorage.clear();

      await expect(apiService.getTodos()).rejects.toThrow();
    });

    it('fetches todo by id', async () => {
      const todo = await apiService.getTodoById(1);

      expect(todo).toHaveProperty('id', 1);
      expect(todo).toHaveProperty('description');
    });

    it('throws error when todo not found', async () => {
      await expect(apiService.getTodoById(9999)).rejects.toThrow();
    });

    it('creates a new todo', async () => {
      const newTodo = await apiService.createTodo({
        description: 'New test todo that is long enough to pass validation',
        categoryId: 1,
        deadline: '2025-12-31',
      });

      expect(newTodo).toHaveProperty('id');
      expect(newTodo.description).toBe('New test todo that is long enough to pass validation');
      expect(newTodo.isCompleted).toBe(false);
    });

    it('updates existing todo', async () => {
      await expect(
        apiService.updateTodo(1, {
          description: 'Updated description that is long enough',
          isCompleted: true,
          deadline: '2025-12-31',
        })
      ).resolves.not.toThrow();
    });

    it('deletes todo', async () => {
      await expect(apiService.deleteTodo(1)).resolves.not.toThrow();
    });

    it('throws error when updating non-existent todo', async () => {
      await expect(
        apiService.updateTodo(9999, {
          description: 'This todo does not exist but has long description',
          isCompleted: true,
        })
      ).rejects.toThrow();
    });

    it('throws error when deleting non-existent todo', async () => {
      await expect(apiService.deleteTodo(9999)).rejects.toThrow();
    });
  });
});
