using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text.RegularExpressions;


namespace Core.Services.CacheService.MicrosoftInMemory;

public class MemoryCacheService : IMemoryCacheService
{
    private readonly IMemoryCache _cache;
    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T Get<T>(string key)
    {
        return _cache.Get<T>(key);
    }

    public object Get(string key)
    {
        return _cache.Get(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="duration">if you wd'nt used duration send -1 </param>
    public void Add(string key, object data, DateTimeOffset? duration)
    {

        if (duration.HasValue && duration.Value != DateTimeOffset.MinValue)
        {
            var opt = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = duration
            };
            _cache.Set(key, data, opt);
        }
        else
            _cache.Set(key, data);
    }

    public bool IsAdd(string key)
    {
        var value = _cache.TryGetValue(key, out _);
        return value;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public List<ICacheEntry> GetAll()
    {
        List<ICacheEntry> result = new();
        var _coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.Instance | BindingFlags.NonPublic);
        var _coherentStateValue = _coherentState.GetValue(_cache);
        var entryCollection = _coherentStateValue.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_coherentStateValue, null);

        var cacheEntriesCollection = entryCollection as dynamic;
        foreach (var cacheItem in cacheEntriesCollection)
            result.Add(cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null));

        return result;
    }

    public int GetCount()
    {
        var _coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.Instance | BindingFlags.NonPublic);
        var _coherentStateValue = _coherentState.GetValue(_cache);
        // _cacheSize

        int result = (int)_coherentStateValue.GetType().GetProperty("Count", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_coherentStateValue, null);


        return result;
    }


    public long CacheSize()
    {
        var _coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.Instance | BindingFlags.NonPublic);
        var _coherentStateValue = _coherentState.GetValue(_cache);
        long result = (long)_coherentStateValue.GetType().GetField("_cacheSize", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_coherentStateValue);


        return result;
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
    public List<ICacheEntry> GetByPattern(string pattern)
    {
        List<ICacheEntry> result = new();
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
        result = cacheCollectionValues.Where(d => regex.IsMatch(d.Key.ToString())).ToList();
        return result;
    }
}
