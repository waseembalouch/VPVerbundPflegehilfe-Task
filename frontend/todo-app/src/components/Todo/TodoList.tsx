import { useState, useMemo, useCallback, useTransition } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useTodos } from '../../hooks/useTodos';
import { TodoItem } from './TodoItem';
import type { FilterType, TodoDto } from '../../types';
import './Todo.css';

function TodoList() {
  const { user, logout } = useAuth();
  const { todos, isLoading, error, createTodo, toggleTodo, deleteTodo } = useTodos();
  const [newTodo, setNewTodo] = useState('');
  const [deadline, setDeadline] = useState(() => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split('T')[0];
  });
  const [filter, setFilter] = useState<FilterType>('All');
  const [localError, setLocalError] = useState('');
  const [isPending, startTransition] = useTransition();

  const handleToggleTodo = useCallback((todo: TodoDto) => {
    toggleTodo(todo);
  }, [toggleTodo]);

  const handleDeleteTodo = useCallback((id: number) => {
    deleteTodo(id);
  }, [deleteTodo]);

  const handleAddTodo = useCallback(async () => {
    if (newTodo.length < 10) {
      setLocalError('Task must be at least 10 characters long');
      return;
    }

    try {
      await createTodo({
        description: newTodo,
        deadline: deadline || null,
      });
      setNewTodo('');
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      setDeadline(tomorrow.toISOString().split('T')[0]);
      setLocalError('');
    } catch (err) {
      // Error handled by useTodos hook
    }
  }, [newTodo, deadline, createTodo]);

  const handleTodoInputChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    setNewTodo(e.target.value);
    setLocalError('');
  }, []);

  const handleKeyDown = useCallback((e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') handleAddTodo();
  }, [handleAddTodo]);

  const handleDeadlineChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    setDeadline(e.target.value);
  }, []);

  const filteredTodos = useMemo(() => {
    return todos.filter((todo) => {
      if (filter === 'Active') return !todo.isCompleted;
      if (filter === 'Completed') return todo.isCompleted;
      return true;
    });
  }, [todos, filter]);

  const activeCount = useMemo(() => {
    return todos.filter((t) => !t.isCompleted).length;
  }, [todos]);

  if (isLoading) {
    return (
      <div className="app">
        <div className="loading">Loading todos...</div>
      </div>
    );
  }

  return (
    <div className="app">
      <header>
        <div className="user-info">
          <span className="username">Welcome, {user?.username}!</span>
          <button onClick={logout} className="logout-btn">
            Logout
          </button>
        </div>
        <h1>todos</h1>
      </header>

      <div className="todo-container">
        <div className="input-section">
          <input
            type="text"
            className="todo-input"
            placeholder="What needs to be done?"
            value={newTodo}
            onChange={handleTodoInputChange}
            onKeyDown={handleKeyDown}
          />
          <input
            type="date"
            className="deadline-input"
            value={deadline}
            onChange={handleDeadlineChange}
          />
          <button onClick={handleAddTodo} className="add-btn">
            Add
          </button>
        </div>

        {(error || localError) && (
          <div className="error-message">{error || localError}</div>
        )}

        <ul className="todo-list" style={{ opacity: isPending ? 0.7 : 1 }}>
          {filteredTodos.map((todo) => (
            <TodoItem
              key={todo.id}
              todo={todo}
              onToggle={handleToggleTodo}
              onDelete={handleDeleteTodo}
            />
          ))}
        </ul>

        {filteredTodos.length === 0 && (
          <div className="empty-state">
            {filter === 'All' && 'No todos yet. Add one above!'}
            {filter === 'Active' && 'No active todos'}
            {filter === 'Completed' && 'No completed todos'}
          </div>
        )}

        <div className="footer">
          <span className="item-count">{activeCount} items left</span>
          <div className="filters">
            <button
              className={filter === 'All' ? 'active' : ''}
              onClick={() => startTransition(() => setFilter('All'))}
            >
              All
            </button>
            <button
              className={filter === 'Active' ? 'active' : ''}
              onClick={() => startTransition(() => setFilter('Active'))}
            >
              Active
            </button>
            <button
              className={filter === 'Completed' ? 'active' : ''}
              onClick={() => startTransition(() => setFilter('Completed'))}
            >
              Completed
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default TodoList;
