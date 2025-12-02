import { Component, type ReactNode, type ErrorInfo } from 'react';

interface ErrorBoundaryProps {
  children: ReactNode;
  fallback?: ReactNode;
}

interface ErrorBoundaryState {
  hasError: boolean;
  error: Error | null;
}

/**
 * Error Boundary component to catch and handle React errors gracefully
 * Prevents the entire app from crashing when an error occurs in a child component
 */
export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    // Update state so the next render will show the fallback UI
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
    // Log the error to an error reporting service
    console.error('ErrorBoundary caught an error:', error, errorInfo);
  }

  handleReset = (): void => {
    this.setState({ hasError: false, error: null });
  };

  render(): ReactNode {
    if (this.state.hasError) {
      // Custom fallback UI if provided
      if (this.props.fallback) {
        return this.props.fallback;
      }

      // Default fallback UI
      return (
        <div style={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh',
          padding: '20px',
          textAlign: 'center',
          backgroundColor: '#f8f9fa'
        }}>
          <div style={{
            maxWidth: '600px',
            padding: '40px',
            backgroundColor: 'white',
            borderRadius: '8px',
            boxShadow: '0 2px 8px rgba(0,0,0,0.1)'
          }}>
            <h1 style={{ color: '#e53e3e', marginBottom: '16px' }}>
              Oops! Something went wrong
            </h1>
            <p style={{ color: '#718096', marginBottom: '24px' }}>
              We're sorry for the inconvenience. The application encountered an unexpected error.
            </p>
            {this.state.error && (
              <details style={{
                marginBottom: '24px',
                padding: '16px',
                backgroundColor: '#f7fafc',
                borderRadius: '4px',
                textAlign: 'left'
              }}>
                <summary style={{ cursor: 'pointer', fontWeight: 'bold', marginBottom: '8px' }}>
                  Error Details
                </summary>
                <pre style={{
                  fontSize: '12px',
                  color: '#c53030',
                  overflow: 'auto',
                  whiteSpace: 'pre-wrap'
                }}>
                  {this.state.error.toString()}
                </pre>
              </details>
            )}
            <button
              onClick={this.handleReset}
              style={{
                padding: '12px 24px',
                backgroundColor: '#4299e1',
                color: 'white',
                border: 'none',
                borderRadius: '6px',
                fontSize: '16px',
                cursor: 'pointer',
                fontWeight: '500',
                transition: 'background-color 0.2s'
              }}
              onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#3182ce'}
              onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#4299e1'}
            >
              Try Again
            </button>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}
