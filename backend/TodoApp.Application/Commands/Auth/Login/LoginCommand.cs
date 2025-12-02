using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Commands.Auth.Login;

public record LoginCommand(
    string Username,
    string Password) : IRequest<AuthResponseDto>;
