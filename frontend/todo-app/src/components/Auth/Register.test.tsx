import { describe, it, expect, vi } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../test/utils/test-utils';
import Register from './Register';

describe('Register Component', () => {
  it('renders registration form with all fields', () => {
    const mockSwitch = vi.fn();
    render(<Register onSwitchToLogin={mockSwitch} />);

    expect(screen.getByPlaceholderText(/choose a username/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/enter your email/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/choose a password/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/confirm your password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /create account/i })).toBeInTheDocument();
  });

  it('validates password match', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Register onSwitchToLogin={mockSwitch} />);

    const usernameInput = screen.getByPlaceholderText(/choose a username/i);
    const emailInput = screen.getByPlaceholderText(/enter your email/i);
    const passwordInput = screen.getByPlaceholderText(/choose a password/i);
    const confirmInput = screen.getByPlaceholderText(/confirm your password/i);
    const registerButton = screen.getByRole('button', { name: /create account/i });

    await user.type(usernameInput, 'newuser');
    await user.type(emailInput, 'new@example.com');
    await user.type(passwordInput, 'password123');
    await user.type(confirmInput, 'password456');
    await user.click(registerButton);

    await waitFor(() => {
      expect(screen.getByText(/passwords do not match/i)).toBeInTheDocument();
    });
  });

  it('validates password length', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Register onSwitchToLogin={mockSwitch} />);

    const usernameInput = screen.getByPlaceholderText(/choose a username/i);
    const emailInput = screen.getByPlaceholderText(/enter your email/i);
    const passwordInput = screen.getByPlaceholderText(/choose a password/i);
    const confirmInput = screen.getByPlaceholderText(/confirm your password/i);
    const registerButton = screen.getByRole('button', { name: /create account/i });

    await user.type(usernameInput, 'newuser');
    await user.type(emailInput, 'new@example.com');
    await user.type(passwordInput, 'short');
    await user.type(confirmInput, 'short');
    await user.click(registerButton);

    await waitFor(() => {
      expect(screen.getByText(/password must be at least 6 characters/i)).toBeInTheDocument();
    });
  });

  it('successfully registers with valid data', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Register onSwitchToLogin={mockSwitch} />);

    const usernameInput = screen.getByPlaceholderText(/choose a username/i);
    const emailInput = screen.getByPlaceholderText(/enter your email/i);
    const passwordInput = screen.getByPlaceholderText(/choose a password/i);
    const confirmInput = screen.getByPlaceholderText(/confirm your password/i);
    const registerButton = screen.getByRole('button', { name: /create account/i });

    await user.type(usernameInput, 'newuser');
    await user.type(emailInput, 'new@example.com');
    await user.type(passwordInput, 'password123');
    await user.type(confirmInput, 'password123');
    await user.click(registerButton);

    // Registration should succeed (no validation errors)
    await waitFor(() => {
      expect(screen.queryByText(/passwords do not match/i)).not.toBeInTheDocument();
      expect(screen.queryByText(/password must be at least/i)).not.toBeInTheDocument();
    });
  });

  it('displays error for existing username', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Register onSwitchToLogin={mockSwitch} />);

    const usernameInput = screen.getByPlaceholderText(/choose a username/i);
    const emailInput = screen.getByPlaceholderText(/enter your email/i);
    const passwordInput = screen.getByPlaceholderText(/choose a password/i);
    const confirmInput = screen.getByPlaceholderText(/confirm your password/i);
    const registerButton = screen.getByRole('button', { name: /create account/i });

    await user.type(usernameInput, 'existinguser');
    await user.type(emailInput, 'test@example.com');
    await user.type(passwordInput, 'password123');
    await user.type(confirmInput, 'password123');
    await user.click(registerButton);

    await waitFor(() => {
      expect(screen.getByText(/username already exists/i)).toBeInTheDocument();
    });
  });

  it('calls onSwitchToLogin when clicking login link', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Register onSwitchToLogin={mockSwitch} />);

    const loginLink = screen.getByText(/sign in here/i);
    await user.click(loginLink);

    expect(mockSwitch).toHaveBeenCalledTimes(1);
  });
});
