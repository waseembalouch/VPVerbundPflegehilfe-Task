using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Infrastructure.Caching;

public class CachedTodoRepository : ITodoRepository
{
    private readonly ITodoRepository _inner;
    private readonly ICacheService _cache;
    private const string CacheKeyPrefix = "todo_";
    private const string AllTodosCacheKey = "todos_all";

    public CachedTodoRepository(ITodoRepository inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        var cachedTodo = _cache.Get<TodoItem>(cacheKey);

        if (cachedTodo != null)
            return cachedTodo;

        var todo = await _inner.GetByIdAsync(id, cancellationToken);
        if (todo != null)
        {
            _cache.Set(cacheKey, todo, TimeSpan.FromMinutes(5));
        }

        return todo;
    }

    public async Task<TodoItem?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}details_{id}";
        var cachedTodo = _cache.Get<TodoItem>(cacheKey);

        if (cachedTodo != null)
            return cachedTodo;

        var todo = await _inner.GetByIdWithDetailsAsync(id, cancellationToken);
        if (todo != null)
        {
            _cache.Set(cacheKey, todo, TimeSpan.FromMinutes(5));
        }

        return todo;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cachedTodos = _cache.Get<IEnumerable<TodoItem>>(AllTodosCacheKey);

        if (cachedTodos != null)
            return cachedTodos;

        var todos = await _inner.GetAllAsync(cancellationToken);
        _cache.Set(AllTodosCacheKey, todos, TimeSpan.FromMinutes(5));

        return todos;
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"todos-user-{userId}";
        var cachedTodos = _cache.Get<IEnumerable<TodoItem>>(cacheKey);

        if (cachedTodos != null)
            return cachedTodos;

        var todos = await _inner.GetByUserIdAsync(userId, cancellationToken);
        _cache.Set(cacheKey, todos, TimeSpan.FromMinutes(5));

        return todos;
    }

    public async Task<IEnumerable<TodoItem>> GetByCompletionStatusAsync(bool isCompleted, CancellationToken cancellationToken = default)
    {
        return await _inner.GetByCompletionStatusAsync(isCompleted, cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        return await _inner.GetOverdueAsync(cancellationToken);
    }

    public async Task<TodoItem> AddAsync(TodoItem entity, CancellationToken cancellationToken = default)
    {
        var result = await _inner.AddAsync(entity, cancellationToken);
        InvalidateCache();
        return result;
    }

    public void Update(TodoItem entity)
    {
        _inner.Update(entity);
        InvalidateCache(entity.Id);
    }

    public void Delete(TodoItem entity)
    {
        _inner.Delete(entity);
        InvalidateCache(entity.Id);
    }

    private void InvalidateCache(int? id = null)
    {
        _cache.RemoveByPattern(CacheKeyPrefix);
        _cache.Remove(AllTodosCacheKey);
    }
}
