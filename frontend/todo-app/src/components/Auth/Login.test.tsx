import { describe, it, expect, vi } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../test/utils/test-utils';
import Login from './Login';

describe('Login Component', () => {
  it('renders login form with all fields', () => {
    const mockSwitch = vi.fn();
    render(<Login onSwitchToRegister={mockSwitch} />);

    expect(screen.getByPlaceholderText(/username/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument();
    expect(screen.getByText(/don't have an account/i)).toBeInTheDocument();
  });

  it('allows user to type in input fields', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Login onSwitchToRegister={mockSwitch} />);

    const usernameInput = screen.getByPlaceholderText(/username/i);
    const passwordInput = screen.getByPlaceholderText(/password/i);

    await user.type(usernameInput, 'testuser');
    await user.type(passwordInput, 'password123');

    expect(usernameInput).toHaveValue('testuser');
    expect(passwordInput).toHaveValue('password123');
  });

  it('submits form with correct credentials', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Login onSwitchToRegister={mockSwitch} />);

    const usernameInput = screen.getByPlaceholderText(/username/i);
    const passwordInput = screen.getByPlaceholderText(/password/i);
    const loginButton = screen.getByRole('button', { name: /sign in/i });

    await user.type(usernameInput, 'testuser');
    await user.type(passwordInput, 'password123');
    await user.click(loginButton);

    // Wait for login to complete (no errors should appear)
    await waitFor(() => {
      expect(screen.queryByText(/invalid/i)).not.toBeInTheDocument();
    }, { timeout: 2000 });
  });

  it('displays error message on invalid credentials', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Login onSwitchToRegister={mockSwitch} />);

    const usernameInput = screen.getByPlaceholderText(/username/i);
    const passwordInput = screen.getByPlaceholderText(/password/i);
    const loginButton = screen.getByRole('button', { name: /sign in/i });

    await user.type(usernameInput, 'wronguser');
    await user.type(passwordInput, 'wrongpass');
    await user.click(loginButton);

    await waitFor(() => {
      expect(screen.getByText(/invalid username or password/i)).toBeInTheDocument();
    });
  });

  it('clears error on new form submission', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Login onSwitchToRegister={mockSwitch} />);

    const usernameInput = screen.getByPlaceholderText(/username/i);
    const passwordInput = screen.getByPlaceholderText(/password/i);
    const loginButton = screen.getByRole('button', { name: /sign in/i });

    // First, create an error
    await user.type(usernameInput, 'wronguser');
    await user.type(passwordInput, 'wrongpass');
    await user.click(loginButton);

    await waitFor(() => {
      expect(screen.getByText(/invalid username or password/i)).toBeInTheDocument();
    });

    // Submit with correct credentials
    await user.clear(usernameInput);
    await user.clear(passwordInput);
    await user.type(usernameInput, 'testuser');
    await user.type(passwordInput, 'password123');
    await user.click(loginButton);

    // Error should be cleared and login successful
    await waitFor(() => {
      expect(screen.queryByText(/invalid username or password/i)).not.toBeInTheDocument();
    }, { timeout: 3000 });
  });

  it('calls onSwitchToRegister when clicking register link', async () => {
    const user = userEvent.setup();
    const mockSwitch = vi.fn();
    render(<Login onSwitchToRegister={mockSwitch} />);

    const registerLink = screen.getByText(/register/i);
    await user.click(registerLink);

    expect(mockSwitch).toHaveBeenCalledTimes(1);
  });
});
