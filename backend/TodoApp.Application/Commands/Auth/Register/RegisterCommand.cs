using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Commands.Auth.Register;

public record RegisterCommand(
    string Username,
    string Email,
    string Password) : IRequest<AuthResponseDto>;
