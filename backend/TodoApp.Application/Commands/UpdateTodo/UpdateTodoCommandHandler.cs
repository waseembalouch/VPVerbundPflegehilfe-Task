using MediatR;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Commands.UpdateTodo;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTodoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _unitOfWork.Todos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TodoItem), request.Id);

        if (todo.UserId != request.UserId)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        todo.Description = request.Description;
        todo.IsCompleted = request.IsCompleted;
        todo.Deadline = request.Deadline;
        todo.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Todos.Update(todo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
