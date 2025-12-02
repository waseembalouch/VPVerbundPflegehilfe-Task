using Microsoft.Extensions.Caching.Memory;
using TodoApp.Application.Interfaces;

namespace TodoApp.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> _cacheKeys = new();
    private readonly object _lock = new();

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T? Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        _cache.Set(key, value, options);

        lock (_lock)
        {
            _cacheKeys.Add(key);
        }
    }

    public void Remove(string key)
    {
        _cache.Remove(key);

        lock (_lock)
        {
            _cacheKeys.Remove(key);
        }
    }

    public void RemoveByPattern(string pattern)
    {
        lock (_lock)
        {
            var keysToRemove = _cacheKeys.Where(k => k.Contains(pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _cacheKeys.Remove(key);
            }
        }
    }
}
