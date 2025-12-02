using TodoApp.API.Endpoints;
using TodoApp.API.Middleware;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Seeding;

namespace TodoApp.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "TodoApp API");
            });
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseCors("TodoAppCorsPolicy");
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapTodoEndpoints();
        app.MapAuthEndpoints();

        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
           .WithName("HealthCheck");

        return app;
    }

    public static WebApplication EnsureDatabaseCreated(this WebApplication app)
    {
        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

            if (app.Environment.IsDevelopment())
            {
                db.Database.EnsureDeleted();
            }

            db.Database.EnsureCreated();
        }

        return app;
    }

    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

            try
            {
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        return app;
    }
}
