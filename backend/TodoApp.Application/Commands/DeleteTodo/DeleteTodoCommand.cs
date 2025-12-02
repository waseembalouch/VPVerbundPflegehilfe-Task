using MediatR;

namespace TodoApp.Application.Commands.DeleteTodo;

public record DeleteTodoCommand(int Id, int UserId) : IRequest<Unit>;
