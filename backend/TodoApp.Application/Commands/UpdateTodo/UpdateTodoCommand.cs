using MediatR;

namespace TodoApp.Application.Commands.UpdateTodo;

public record UpdateTodoCommand(
    int Id,
    int UserId,
    string Description,
    bool IsCompleted,
    DateTime? Deadline) : IRequest<Unit>;
