using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Commands.CreateTodo;

public record CreateTodoCommand(
    string Description,
    int UserId,
    DateTime? Deadline) : IRequest<TodoDto>;
