namespace AspNotes.Interfaces;

/// <summary>
/// Provides application-wide caching services.
/// </summary>
public interface IAppCache
{
    /// <summary>
    /// Updates the cache with a new item.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="value">The value to be cached.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    void UpdateCache<T>(string cacheKey, T value);

    /// <summary>
    /// Retrieves an item from the cache.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <returns>The cached item, or the default value of <typeparamref name="T"/> if the key is not found.</returns>
    T? GetCache<T>(string cacheKey);

    /// <summary>
    /// Removes an item from the cache.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    void RemoveCache(string cacheKey);
}
