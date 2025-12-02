namespace TodoApp.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITodoRepository Todos { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
