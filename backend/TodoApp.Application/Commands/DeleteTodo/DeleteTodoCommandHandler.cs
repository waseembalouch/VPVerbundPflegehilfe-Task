using MediatR;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Commands.DeleteTodo;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTodoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _unitOfWork.Todos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TodoItem), request.Id);

        if (todo.UserId != request.UserId)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        _unitOfWork.Todos.Delete(todo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
