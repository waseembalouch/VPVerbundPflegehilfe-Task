import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import { apiService, isApiError } from '../services/api';
import type { AuthResponseDto, LoginRequest, RegisterRequest } from '../types';

interface AuthContextType {
  user: AuthResponseDto | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
  error: string | null;
  clearError: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthResponseDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // Check for existing token on mount
    const token = localStorage.getItem('token');
    const username = localStorage.getItem('username');
    const email = localStorage.getItem('email');

    if (token && username && email) {
      setUser({ token, username, email });
    }
    setIsLoading(false);
  }, []);

  const login = async (credentials: LoginRequest) => {
    try {
      setError(null);
      setIsLoading(true);
      const response = await apiService.login(credentials);

      localStorage.setItem('token', response.token);
      localStorage.setItem('username', response.username);
      localStorage.setItem('email', response.email);

      setUser(response);
    } catch (err) {
      const message = isApiError(err) ? err.message : 'Login failed';
      setError(message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (data: RegisterRequest) => {
    try {
      setError(null);
      setIsLoading(true);
      const response = await apiService.register(data);

      localStorage.setItem('token', response.token);
      localStorage.setItem('username', response.username);
      localStorage.setItem('email', response.email);

      setUser(response);
    } catch (err) {
      const message = isApiError(err) ? err.message : 'Registration failed';
      setError(message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('email');
    setUser(null);
    setError(null);
  };

  const clearError = () => setError(null);

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        isLoading,
        login,
        register,
        logout,
        error,
        clearError,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
