using AutoMapper;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Queries.GetAllTodos;

public class GetAllTodosQueryHandler : IRequestHandler<GetAllTodosQuery, IEnumerable<TodoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTodosQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TodoDto>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        var todos = await _unitOfWork.Todos.GetByUserIdAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<TodoDto>>(todos);
    }
}
