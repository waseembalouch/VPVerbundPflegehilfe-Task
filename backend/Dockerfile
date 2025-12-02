# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["TodoApp.API/TodoApp.API.csproj", "TodoApp.API/"]
COPY ["TodoApp.Application/TodoApp.Application.csproj", "TodoApp.Application/"]
COPY ["TodoApp.Domain/TodoApp.Domain.csproj", "TodoApp.Domain/"]
COPY ["TodoApp.Infrastructure/TodoApp.Infrastructure.csproj", "TodoApp.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "TodoApp.API/TodoApp.API.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/TodoApp.API"
RUN dotnet build "TodoApp.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "TodoApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create a non-root user and data directory
RUN useradd -m -s /bin/bash appuser && \
    mkdir -p /app/data && \
    chown -R appuser /app
USER appuser

# Copy published files
COPY --from=publish /app/publish .

# Expose port
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

# Run the application
ENTRYPOINT ["dotnet", "TodoApp.API.dll"]
