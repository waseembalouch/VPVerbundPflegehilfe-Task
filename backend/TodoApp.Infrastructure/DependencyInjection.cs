using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Interfaces;
using TodoApp.Infrastructure.Authentication;
using TodoApp.Infrastructure.Caching;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Seeding;

namespace TodoApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool skipDbContext = false)
    {
        if (!skipDbContext)
        {
            services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(TodoDbContext).Assembly.FullName)));
        }

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.Decorate<ITodoRepository, CachedTodoRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();

        services.AddScoped<DatabaseSeeder>();

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "TodoApp")
            .WriteTo.Console()
            .WriteTo.File("logs/app-.txt", rollingInterval: Serilog.RollingInterval.Day)
            .CreateLogger();

        services.AddSerilog();

        return services;
    }
}
