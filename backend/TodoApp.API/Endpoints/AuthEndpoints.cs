using MediatR;
using TodoApp.Application.Commands.Auth.Login;
using TodoApp.Application.Commands.Auth.Register;

namespace TodoApp.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", async (RegisterRequest request, IMediator mediator, CancellationToken ct) =>
        {
            var command = new RegisterCommand(
                request.Username,
                request.Email,
                request.Password);

            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/auth/user/{result.Id}", result);
        })
        .WithName("Register")
        .Produces(201)
        .Produces(400)
        .WithOpenApi();

        group.MapPost("/login", async (LoginRequest request, IMediator mediator, CancellationToken ct) =>
        {
            var command = new LoginCommand(
                request.Username,
                request.Password);

            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("Login")
        .Produces(200)
        .Produces(400)
        .WithOpenApi();
    }
}

public record RegisterRequest(
    string Username,
    string Email,
    string Password);

public record LoginRequest(
    string Username,
    string Password);
