using AutoMapper;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Commands.CreateTodo;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTodoCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TodoDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todoItem = new TodoItem
        {
            Description = request.Description,
            UserId = request.UserId,
            Deadline = request.Deadline,
            IsCompleted = false
        };

        var createdTodo = await _unitOfWork.Todos.AddAsync(todoItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var todoWithDetails = await _unitOfWork.Todos.GetByIdWithDetailsAsync(createdTodo.Id, cancellationToken);

        return _mapper.Map<TodoDto>(todoWithDetails);
    }
}
