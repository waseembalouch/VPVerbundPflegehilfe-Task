// =============================================================================
// Vite Configuration
// =============================================================================
// https://vite.dev/config/
// =============================================================================

import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'

export default defineConfig({
  // Plugins
  // ---------------------------------------------------------------------------
  plugins: [
    react(), // Enable React Fast Refresh and JSX transformation
  ],

  // Build Configuration
  // ---------------------------------------------------------------------------
  build: {
    // Output directory for production build
    outDir: 'dist',

    // Generate sourcemaps for production (set to false to disable)
    sourcemap: false,

    // Minification strategy (esbuild is faster than terser)
    minify: 'esbuild',

    // Target browsers (modern browsers only)
    target: 'es2015',

    // Rollup options for advanced bundling configuration
    rollupOptions: {
      output: {
        // Let Vite handle chunking automatically
        manualChunks: undefined,
      },
    },

    // Chunk size warnings (in KB)
    chunkSizeWarningLimit: 500,
  },

  // Development Server Configuration
  // ---------------------------------------------------------------------------
  server: {
    port: 5173,
    host: true,
    open: false,
  },

  // Preview Server Configuration
  // ---------------------------------------------------------------------------
  preview: {
    port: 4173,
    host: true,
  },

  // Test Configuration (Vitest)
  // ---------------------------------------------------------------------------
  test: {
    // Enable global test APIs (describe, it, expect, etc.)
    globals: true,

    // Simulate DOM environment for React component testing
    environment: 'jsdom',

    // Setup file to run before each test
    setupFiles: './src/test/setup.ts',

    // Process CSS imports in tests
    css: true,

    // Coverage configuration
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      exclude: [
        'node_modules/',
        'src/test/',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData',
        'dist/',
      ],
    },
  },
})
