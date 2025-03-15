using AspNotes.Common;
using Microsoft.Extensions.Caching.Memory;

namespace AspNotes.Tests.Common;

public class AppCacheTests
{
    private readonly IMemoryCache memoryCache;
    private readonly AppCache appCache;

    public AppCacheTests()
    {
        memoryCache = new MemoryCache(new MemoryCacheOptions());
        appCache = new AppCache(memoryCache);
    }

    [Fact]
    public void UpdateCache_ShouldStoreItemInCache()
    {
        // Arrange
        var cacheKey = "testKey";
        var value = "testValue";

        // Act
        appCache.UpdateCache(cacheKey, value);

        // Assert
        Assert.True(memoryCache.TryGetValue(cacheKey, out var cachedValue));
        Assert.NotNull(cachedValue);
        Assert.Equal(value, cachedValue);
    }

    [Fact]
    public void GetCache_ShouldRetrieveItemFromCache()
    {
        // Arrange
        var cacheKey = "testKey";
        var value = "testValue";
        memoryCache.Set(cacheKey, value);

        // Act
        var result = appCache.GetCache<string>(cacheKey);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void GetCache_ShouldReturnNullIfItemNotInCache()
    {
        // Arrange
        var cacheKey = "nonExistentKey";

        // Act
        var result = appCache.GetCache<string>(cacheKey);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RemoveCache_ShouldRemoveItemFromCache()
    {
        // Arrange
        var cacheKey = "testKey";
        var value = "testValue";
        memoryCache.Set(cacheKey, value);

        // Act
        appCache.RemoveCache(cacheKey);

        // Assert
        Assert.False(memoryCache.TryGetValue(cacheKey, out _));
    }
}
