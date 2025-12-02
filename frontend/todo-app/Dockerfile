# =============================================================================
# Multi-Stage Dockerfile for React + Vite Application
# =============================================================================
# Stage 1: Build the application
# Stage 2: Serve with nginx
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1: Build
# -----------------------------------------------------------------------------
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Install dependencies first (better layer caching)
# Only copy package files to leverage Docker layer caching
COPY package.json package-lock.json ./

# Install dependencies using npm ci for reproducible builds
# --ignore-scripts: Skip pre/post install scripts for security
RUN npm ci --ignore-scripts

# Copy application source code
# This is done after npm ci to maximize cache hits
COPY . .

# Build the application for production
# Environment variables from .env.production will be used
RUN npm run build

# -----------------------------------------------------------------------------
# Stage 2: Production
# -----------------------------------------------------------------------------
FROM nginx:alpine AS production

# Add metadata labels
LABEL maintainer="your-email@example.com"
LABEL description="TodoApp Frontend - React SPA with Nginx"

# Set working directory
WORKDIR /usr/share/nginx/html

# Remove default nginx static assets
RUN rm -rf ./*

# Copy built assets from build stage
# Only copy the dist folder to minimize image size
COPY --from=build /app/dist .

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Create non-root user for running nginx (security best practice)
RUN addgroup -g 1001 -S nginx-user && \
    adduser -S -D -H -u 1001 -h /var/cache/nginx -s /sbin/nologin -G nginx-user -g nginx-user nginx-user && \
    chown -R nginx-user:nginx-user /usr/share/nginx/html && \
    chown -R nginx-user:nginx-user /var/cache/nginx && \
    chmod -R 755 /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Health check to ensure container is running properly
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost/health || exit 1

# Run nginx in foreground
CMD ["nginx", "-g", "daemon off;"]
