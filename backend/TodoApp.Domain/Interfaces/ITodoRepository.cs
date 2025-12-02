using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces;

public interface ITodoRepository : IRepository<TodoItem>
{
    Task<IEnumerable<TodoItem>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> GetByCompletionStatusAsync(bool isCompleted, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task<TodoItem?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
