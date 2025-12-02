import { useState } from 'react';
import { useAuth } from './context/AuthContext';
import './App.css';
import Login from './components/Auth/Login';
import Register from './components/Auth/Register';
import TodoList from './components/Todo/TodoList';

function App() {
  const { isAuthenticated, isLoading } = useAuth();
  const [showLogin, setShowLogin] = useState(true);

  if (isLoading) {
    return (
      <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh'
      }}>
        <div>Loading...</div>
      </div>
    );
  }

  if (!isAuthenticated) {
    if (showLogin) {
      return <Login onSwitchToRegister={() => setShowLogin(false)} />;
    } else {
      return <Register onSwitchToLogin={() => setShowLogin(true)} />;
    }
  }

  return <TodoList />;
}

export default App;
