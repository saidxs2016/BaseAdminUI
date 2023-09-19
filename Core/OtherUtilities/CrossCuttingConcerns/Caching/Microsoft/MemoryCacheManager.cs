using Core.OtherUtilities.IoC;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Core.OtherUtilities.CrossCuttingConcerns.Caching.Microsoft;

public class MemoryCacheManager : ICacheManager
{
    private readonly IMemoryCache _cache;
    public MemoryCacheManager()
    {
        _cache = ServiceTool.ServiceProvider.GetService<IMemoryCache>();
    }
    public T Get<T>(string key)
    {
        return _cache.Get<T>(key);
    }

    public object Get(string key)
    {
        return _cache.Get(key);
    }

    public void Add(string key, object data, int duration)
    {
        _cache.Set(key, data, TimeSpan.FromMinutes(duration));
    }

    public bool IsAdd(string key)
    {
        return _cache.TryGetValue(key, out _);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
    public void RemoveByPattern(string pattern)
    {
        var _coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.Instance | BindingFlags.NonPublic);
        var _coherentStateValue = _coherentState.GetValue(_cache);
        var entryCollection = _coherentStateValue.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_coherentStateValue, null);

        var cacheEntriesCollection = entryCollection as dynamic;
        List<ICacheEntry> cacheCollectionValues = new();

        foreach (var cacheItem in cacheEntriesCollection)
        {
            ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
            cacheCollectionValues.Add(cacheItemValue);
        }

        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var keysToRemove = cacheCollectionValues.Where(d => regex.IsMatch(d.Key.ToString())).Select(d => d.Key).ToList();

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
        }
    }
}
