using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Queries.GetTodoById;

public record GetTodoByIdQuery(int Id, int UserId) : IRequest<TodoDto>;
