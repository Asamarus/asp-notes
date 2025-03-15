using AspNotes.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace AspNotes.Common;

/// <summary>
/// Provides application-wide caching services.
/// </summary>
public class AppCache(IMemoryCache cache) : IAppCache
{
    /// <inheritdoc />
    public void UpdateCache<T>(string cacheKey, T value)
    {
        cache.Set(cacheKey, value);
    }

    /// <inheritdoc />
    public T? GetCache<T>(string cacheKey)
    {
        cache.TryGetValue(cacheKey, out T? item);
        return item;
    }

    /// <inheritdoc />
    public void RemoveCache(string cacheKey)
    {
        cache.Remove(cacheKey);
    }
}