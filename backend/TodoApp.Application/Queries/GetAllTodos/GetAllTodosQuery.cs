using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Queries.GetAllTodos;

public record GetAllTodosQuery(int UserId) : IRequest<IEnumerable<TodoDto>>;
