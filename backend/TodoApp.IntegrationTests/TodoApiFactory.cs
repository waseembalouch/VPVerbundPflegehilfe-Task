using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.IntegrationTests;

public class TodoApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDatabase_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });

        builder.ConfigureTestServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TodoDbContext>();

            db.Database.EnsureCreated();

            SeedTestData(db);
        });
    }

    private static void SeedTestData(TodoDbContext context)
    {
        context.SaveChanges();
    }
}
