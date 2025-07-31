using HcAgents.Domain.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace HcAgents.Infrastructure.Cache;

public class MemoryCacheRepository : IMemoryCacheRepository
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key)
    {
        _memoryCache.TryGetValue(key, out T? value);
        return value;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public void Set<T>(string key, T value, TimeSpan expiry)
    {
        _memoryCache.Set(
            key,
            value,
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry }
        );
    }
}
