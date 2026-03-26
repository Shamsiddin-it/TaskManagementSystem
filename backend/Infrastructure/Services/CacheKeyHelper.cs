using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

public static class CacheKeyHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public static string BuildIdKey(string entity, int id)
    {
        return $"{entity}:id:{id}";
    }

    public static string BuildListKey(string entity, object? filter, PaginationFilter pagination)
    {
        var filterJson = JsonSerializer.Serialize(filter ?? new { }, JsonOptions);
        var pageJson = JsonSerializer.Serialize(new { pagination.Page, pagination.PageSize }, JsonOptions);
        return $"{entity}:list:{filterJson}:{pageJson}";
    }

    public static MemoryCacheEntryOptions DefaultOptions()
    {
        return new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            .SetSlidingExpiration(TimeSpan.FromMinutes(1))
            .SetPriority(CacheItemPriority.High);
    }
}

public static class CacheKeyStore
{
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> Store = new();

    public static void Add(string entity, string key)
    {
        var set = Store.GetOrAdd(entity, _ => new ConcurrentDictionary<string, byte>());
        set.TryAdd(key, 0);
    }

    public static void RemoveEntity(IMemoryCache cache, string entity)
    {
        if (!Store.TryGetValue(entity, out var keys))
            return;

        foreach (var key in keys.Keys)
        {
            cache.Remove(key);
        }

        keys.Clear();
    }

    public static void RemoveKey(IMemoryCache cache, string entity, string key)
    {
        cache.Remove(key);
        if (Store.TryGetValue(entity, out var keys))
        {
            keys.TryRemove(key, out _);
        }
    }
}
