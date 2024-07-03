using Microsoft.Extensions.Caching.Memory;

namespace AspNotes.Core.Common;

/// <summary>
/// Provides application-wide caching services.
/// </summary>
public class AppCache(IMemoryCache cache)
{
    /// <summary>
    /// Updates the cache with a new item.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="value">The value to be cached.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    public void UpdateCache<T>(string cacheKey, T value)
    {
        cache.Set(cacheKey, value);
    }

    /// <summary>
    /// Retrieves an item from the cache.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <returns>The cached item, or the default value of <typeparamref name="T"/> if the key is not found.</returns>
    public T? GetCache<T>(string cacheKey)
    {
        cache.TryGetValue(cacheKey, out T? item);
        return item;
    }

    /// <summary>
    /// Removes an item from the cache.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    public void RemoveCache(string cacheKey)
    {
        cache.Remove(cacheKey);
    }
}
