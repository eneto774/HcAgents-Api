using Microsoft.Extensions.Caching.Memory;

namespace HcAgents.Domain.Abstractions;

public interface IMemoryCacheRepository
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiry);
    void Remove(string key);
}
