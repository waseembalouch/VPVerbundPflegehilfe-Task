import { describe, it, expect, vi } from 'vitest';
import { screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../test/utils/test-utils';
import { TodoItem } from './TodoItem';
import type { TodoDto } from '../../types';

const mockTodo: TodoDto = {
  id: 1,
  description: 'Test todo item',
  isCompleted: false,
  deadline: '2025-12-31',
  createdAt: '2025-11-20T10:00:00Z',
  categoryId: 1,
};

describe('TodoItem Component', () => {
  it('renders todo item with description', () => {
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();

    render(
      <TodoItem todo={mockTodo} onToggle={mockToggle} onDelete={mockDelete} />
    );

    expect(screen.getByText('Test todo item')).toBeInTheDocument();
  });

  it('renders checkbox unchecked for incomplete todo', () => {
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();

    render(
      <TodoItem todo={mockTodo} onToggle={mockToggle} onDelete={mockDelete} />
    );

    const checkbox = screen.getByRole('checkbox');
    expect(checkbox).not.toBeChecked();
  });

  it('renders checkbox checked for completed todo', () => {
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();
    const completedTodo = { ...mockTodo, isCompleted: true };

    render(
      <TodoItem todo={completedTodo} onToggle={mockToggle} onDelete={mockDelete} />
    );

    const checkbox = screen.getByRole('checkbox');
    expect(checkbox).toBeChecked();
  });

  it('displays deadline when provided', () => {
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();

    render(
      <TodoItem todo={mockTodo} onToggle={mockToggle} onDelete={mockDelete} />
    );

    // Check for deadline date (format may vary by locale)
    expect(screen.getByText(/31.*12.*2025|12.*31.*2025/)).toBeInTheDocument();
  });

  it('calls onToggle when checkbox is clicked', async () => {
    const user = userEvent.setup();
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();

    render(
      <TodoItem todo={mockTodo} onToggle={mockToggle} onDelete={mockDelete} />
    );

    const checkbox = screen.getByRole('checkbox');
    await user.click(checkbox);

    expect(mockToggle).toHaveBeenCalledWith(mockTodo);
    expect(mockToggle).toHaveBeenCalledTimes(1);
  });

  it('calls onDelete when delete button is clicked', async () => {
    const user = userEvent.setup();
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();

    render(
      <TodoItem todo={mockTodo} onToggle={mockToggle} onDelete={mockDelete} />
    );

    const deleteButton = screen.getByRole('button');
    await user.click(deleteButton);

    expect(mockDelete).toHaveBeenCalledWith(mockTodo.id);
    expect(mockDelete).toHaveBeenCalledTimes(1);
  });

  it('applies overdue class when deadline has passed', () => {
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();
    const overdueTodo = {
      ...mockTodo,
      deadline: '2020-01-01', // Past date
      isCompleted: false,
    };

    const { container } = render(
      <TodoItem todo={overdueTodo} onToggle={mockToggle} onDelete={mockDelete} />
    );

    const todoItem = container.querySelector('.todo-item');
    expect(todoItem).toHaveClass('overdue');
  });

  it('does not apply overdue class for completed todos', () => {
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();
    const completedOverdueTodo = {
      ...mockTodo,
      deadline: '2020-01-01',
      isCompleted: true,
    };

    const { container } = render(
      <TodoItem todo={completedOverdueTodo} onToggle={mockToggle} onDelete={mockDelete} />
    );

    const todoItem = container.querySelector('.todo-item');
    expect(todoItem).not.toHaveClass('overdue');
  });

  it('does not display deadline when not provided', () => {
    const mockToggle = vi.fn();
    const mockDelete = vi.fn();
    const todoWithoutDeadline = { ...mockTodo, deadline: null };

    render(
      <TodoItem todo={todoWithoutDeadline} onToggle={mockToggle} onDelete={mockDelete} />
    );

    expect(screen.queryByText(/\d{1,2}\/\d{1,2}\/\d{4}/)).not.toBeInTheDocument();
  });
});
