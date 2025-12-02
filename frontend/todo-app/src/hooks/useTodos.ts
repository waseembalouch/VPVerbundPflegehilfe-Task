import { useState, useEffect, useCallback } from 'react';
import { apiService, isApiError } from '../services/api';
import type { TodoDto, CreateTodoRequest, UpdateTodoRequest } from '../types';

export function useTodos() {
  const [todos, setTodos] = useState<TodoDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTodos = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await apiService.getTodos();
      setTodos(data);
    } catch (err) {
      const message = isApiError(err) ? err.message : 'Failed to fetch todos';
      setError(message);
      console.error('Error fetching todos:', err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchTodos();
  }, [fetchTodos]);

  const createTodo = async (todo: CreateTodoRequest): Promise<void> => {
    const optimisticTodo: TodoDto = {
      id: Date.now(),
      description: todo.description,
      isCompleted: false,
      deadline: todo.deadline || null,
      createdAt: new Date().toISOString(),
    };

    setTodos(prev => [...prev, optimisticTodo]);
    setError(null);

    try {
      const newTodo = await apiService.createTodo(todo);
      setTodos(prev => prev.map(t => t.id === optimisticTodo.id ? newTodo : t));
    } catch (err) {
      setTodos(prev => prev.filter(t => t.id !== optimisticTodo.id));
      const message = isApiError(err) ? err.message : 'Failed to create todo';
      setError(message);
      throw err;
    }
  };

  const updateTodo = async (id: number, todo: UpdateTodoRequest): Promise<void> => {
    const previousTodos = [...todos];

    setTodos(prev => prev.map(t =>
      t.id === id
        ? { ...t, ...todo }
        : t
    ));
    setError(null);

    try {
      await apiService.updateTodo(id, todo);
    } catch (err) {
      setTodos(previousTodos);
      const message = isApiError(err) ? err.message : 'Failed to update todo';
      setError(message);
      throw err;
    }
  };

  const deleteTodo = async (id: number): Promise<void> => {
    const previousTodos = [...todos];

    setTodos(prev => prev.filter(t => t.id !== id));
    setError(null);

    try {
      await apiService.deleteTodo(id);
    } catch (err) {
      setTodos(previousTodos);
      const message = isApiError(err) ? err.message : 'Failed to delete todo';
      setError(message);
      throw err;
    }
  };

  const toggleTodo = async (todo: TodoDto): Promise<void> => {
    await updateTodo(todo.id, {
      description: todo.description,
      isCompleted: !todo.isCompleted,
      deadline: todo.deadline,
    });
  };

  return {
    todos,
    isLoading,
    error,
    createTodo,
    updateTodo,
    deleteTodo,
    toggleTodo,
    refetch: fetchTodos,
  };
}
