using TodoApp.Domain.Interfaces;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TodoDbContext _context;
    private ITodoRepository? _todos;
    private IUserRepository? _users;

    public UnitOfWork(TodoDbContext context)
    {
        _context = context;
    }

    public ITodoRepository Todos =>
        _todos ??= new TodoRepository(_context);

    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
