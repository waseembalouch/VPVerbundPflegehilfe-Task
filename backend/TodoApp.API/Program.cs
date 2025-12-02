using Serilog;
using TodoApp.API.Extensions;
using TodoApp.Application;
using TodoApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, skipDbContext: builder.Environment.IsEnvironment("Testing"));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);

var app = builder.Build();

app.EnsureDatabaseCreated()
   .ConfigurePipeline()
   .MapApiEndpoints();

await app.SeedDatabaseAsync();

Log.Information("Starting TodoApp API...");
app.Run();
Log.Information("TodoApp API shut down gracefully");

// Make Program class accessible for integration tests
public partial class Program { }
