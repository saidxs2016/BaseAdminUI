using StackExchange.Redis;
using System.Text.Json;

namespace Core.Services.CacheService.Redis;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redisCon;
    private readonly IDatabase _cache;

    public RedisCacheService(IConnectionMultiplexer redisCon)
    {
        _redisCon = redisCon;
        _cache = redisCon.GetDatabase();
    }

    public IEnumerable<string> GetAllKeys()
    {
        var endpoint = _redisCon.GetEndPoints();
        var server = _redisCon.GetServer(endpoint.First());
        var data = server.Keys();
        return data?.Select(k => k.ToString());
    }

    public bool Clear(string key) => _cache.KeyDelete(key);
    public async Task<bool> ClearAsync(string key) => await _cache.KeyDeleteAsync(key);

    public void ClearAll()
    {
        var endpoints = _redisCon.GetEndPoints(true);
        foreach (var endpoint in endpoints)
        {
            var server = _redisCon.GetServer(endpoint);
            server.FlushAllDatabases();
        }
    }

    public T Get<T>(string key) => JsonSerializer.Deserialize<T>(_cache.StringGet(key));
    public async Task<T> GetAsync<T>(string key)
    {
        var str = await _cache.StringGetAsync(key);
        return JsonSerializer.Deserialize<T>(str);
    }
    public string GetValue(string key) => _cache.StringGet(key);

    public async Task<string> GetValueAsync(string key) => await _cache.StringGetAsync(key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="ExpireTime">as Minuts</param>
    /// <returns></returns>
    public bool SetValue(string key, string value, int expireTime)
    {
        if (expireTime > 0)
            return _cache.StringSet(key, value, TimeSpan.FromMinutes(expireTime));
        else
            return _cache.StringSet(key, value);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="ExpireTime"> as Minuts</param>
    /// <returns></returns>
    public async Task<bool> SetValueAsync(string key, string value, int expireTime)
    {
        if (expireTime > 0)
            return await _cache.StringSetAsync(key, value, TimeSpan.FromMinutes(expireTime));
        else
            return await _cache.StringSetAsync(key, value);

    }

    public T GetOrAdd<T>(string key, Func<T> action, int expireTime) where T : class
    {
        var result = _cache.StringGet(key);
        if (result.IsNull)
        {
            result = JsonSerializer.SerializeToUtf8Bytes(action());
            SetValue(key, result, expireTime);
        }
        return JsonSerializer.Deserialize<T>(result);
    }
    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> action, int expireTime) where T : class
    {
        var result = await _cache.StringGetAsync(key);
        if (result.IsNull)
        {
            result = JsonSerializer.SerializeToUtf8Bytes(await action());
            await SetValueAsync(key, result, expireTime);
        }
        return JsonSerializer.Deserialize<T>(result);
    }

    public bool Any(string key) => _cache.KeyExists(key);
    public async Task<bool> AnyAsync(string key) => await _cache.KeyExistsAsync(key);


}
