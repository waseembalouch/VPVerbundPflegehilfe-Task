import { ReactElement } from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { AuthProvider } from '../../context/AuthContext';

// Custom render function that includes providers
interface CustomRenderOptions extends Omit<RenderOptions, 'wrapper'> {
  withAuth?: boolean;
}

export function renderWithProviders(
  ui: ReactElement,
  { withAuth = true, ...renderOptions }: CustomRenderOptions = {}
) {
  function Wrapper({ children }: { children: React.ReactNode }) {
    if (withAuth) {
      return <AuthProvider>{children}</AuthProvider>;
    }
    return <>{children}</>;
  }

  return render(ui, { wrapper: Wrapper, ...renderOptions });
}

// Re-export everything from React Testing Library
export * from '@testing-library/react';
export { renderWithProviders as render };
