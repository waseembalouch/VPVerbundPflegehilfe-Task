import { memo } from 'react';
import type { TodoDto } from '../../types';

interface TodoItemProps {
  todo: TodoDto;
  onToggle: (todo: TodoDto) => void;
  onDelete: (id: number) => void;
}

// Memoized to prevent re-renders when other items change
export const TodoItem = memo(function TodoItem({ todo, onToggle, onDelete }: TodoItemProps) {
  const isOverdue = (deadline: string | null): boolean => {
    if (!deadline) return false;
    return new Date(deadline) < new Date() && !todo.isCompleted;
  };

  return (
    <li className={`todo-item ${isOverdue(todo.deadline) ? 'overdue' : ''}`}>
      <input
        type="checkbox"
        checked={todo.isCompleted}
        onChange={() => onToggle(todo)}
        className="todo-checkbox"
      />
      <span className={`todo-text ${todo.isCompleted ? 'completed' : ''}`}>
        {todo.description}
      </span>
      {todo.deadline && (
        <span className="deadline">
          {new Date(todo.deadline).toLocaleDateString()}
        </span>
      )}
      <button onClick={() => onDelete(todo.id)} className="delete-btn">
        ×
      </button>
    </li>
  );
});
