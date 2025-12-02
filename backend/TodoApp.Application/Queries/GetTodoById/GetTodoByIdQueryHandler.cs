using AutoMapper;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Queries.GetTodoById;

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTodoByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TodoDto> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _unitOfWork.Todos.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TodoItem), request.Id);

        if (todo.UserId != request.UserId)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        return _mapper.Map<TodoDto>(todo);
    }
}
