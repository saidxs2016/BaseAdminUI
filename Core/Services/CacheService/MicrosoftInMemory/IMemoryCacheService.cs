using Microsoft.Extensions.Caching.Memory;

namespace Core.Services.CacheService.MicrosoftInMemory;

public interface IMemoryCacheService
{
    List<ICacheEntry> GetAll();
    T Get<T>(string key);
    object Get(string key);

    long CacheSize();
    int GetCount();

    void Add(string key, object data, DateTimeOffset? duration);
    bool IsAdd(string key);
    void Remove(string key);
    void RemoveByPattern(string pattern);
    List<ICacheEntry> GetByPattern(string pattern);
}
