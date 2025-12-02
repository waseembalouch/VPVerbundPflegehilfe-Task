using System.Security.Claims;
using MediatR;
using TodoApp.Application.Commands.CreateTodo;
using TodoApp.Application.Commands.DeleteTodo;
using TodoApp.Application.Commands.UpdateTodo;
using TodoApp.Application.Queries.GetAllTodos;
using TodoApp.Application.Queries.GetTodoById;

namespace TodoApp.API.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos")
            .WithTags("Todos")
            .RequireAuthorization();

        group.MapGet("/", async (HttpContext httpContext, IMediator mediator, CancellationToken ct) =>
        {
            var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var todos = await mediator.Send(new GetAllTodosQuery(userId), ct);
            return Results.Ok(todos);
        })
        .WithName("GetAllTodos")
        .Produces(200)
        .WithOpenApi();

        group.MapGet("/{id:int}", async (int id, HttpContext httpContext, IMediator mediator, CancellationToken ct) =>
        {
            var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var todo = await mediator.Send(new GetTodoByIdQuery(id, userId), ct);
            return Results.Ok(todo);
        })
        .WithName("GetTodoById")
        .Produces(200)
        .Produces(404)
        .WithOpenApi();

        group.MapPost("/", async (CreateTodoRequest request, HttpContext httpContext, IMediator mediator, CancellationToken ct) =>
        {
            var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new CreateTodoCommand(
                request.Description,
                userId,
                request.Deadline);

            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/todos/{result.Id}", result);
        })
        .WithName("CreateTodo")
        .Produces(201)
        .Produces(400)
        .WithOpenApi();

        group.MapPut("/{id:int}", async (int id, UpdateTodoRequest request, HttpContext httpContext, IMediator mediator, CancellationToken ct) =>
        {
            var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new UpdateTodoCommand(
                id,
                userId,
                request.Description,
                request.IsCompleted,
                request.Deadline);

            await mediator.Send(command, ct);
            return Results.NoContent();
        })
        .WithName("UpdateTodo")
        .Produces(204)
        .Produces(400)
        .Produces(404)
        .WithOpenApi();

        group.MapDelete("/{id:int}", async (int id, HttpContext httpContext, IMediator mediator, CancellationToken ct) =>
        {
            var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await mediator.Send(new DeleteTodoCommand(id, userId), ct);
            return Results.NoContent();
        })
        .WithName("DeleteTodo")
        .Produces(204)
        .Produces(404)
        .WithOpenApi();
    }
}

public record CreateTodoRequest(
    string Description,
    DateTime? Deadline);

public record UpdateTodoRequest(
    string Description,
    bool IsCompleted,
    DateTime? Deadline);
