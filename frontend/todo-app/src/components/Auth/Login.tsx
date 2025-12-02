import { useState, type FormEvent } from 'react';
import { useAuth } from '../../context/AuthContext';
import './Auth.css';

interface LoginProps {
  onSwitchToRegister: () => void;
}

function Login({ onSwitchToRegister }: LoginProps) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const { login, error, clearError } = useAuth();

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    clearError();

    try {
      await login({ username, password });
    } catch (err) {
      // Error is handled by AuthContext
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>Welcome Back</h2>
        <p className="auth-subtitle">Sign in to manage your todos</p>

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="form-group">
            <label htmlFor="username">Username</label>
            <input
              id="username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Enter your username"
              required
              autoComplete="username"
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter your password"
              required
              autoComplete="current-password"
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button type="submit" className="auth-button">
            Sign In
          </button>
        </form>

        <p className="auth-switch">
          Don't have an account?{' '}
          <button onClick={onSwitchToRegister} className="link-button">
            Register here
          </button>
        </p>
      </div>
    </div>
  );
}

export default Login;
