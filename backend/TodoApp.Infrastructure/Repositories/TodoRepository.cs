using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repositories;

public class TodoRepository : Repository<TodoItem>, ITodoRepository
{
    public TodoRepository(TodoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetByCompletionStatusAsync(bool isCompleted, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.IsCompleted == isCompleted)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await DbSet
            .Where(t => t.Deadline.HasValue && t.Deadline.Value < now && !t.IsCompleted)
            .OrderBy(t => t.Deadline)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
