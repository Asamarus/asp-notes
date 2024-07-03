using AspNotes.Core.Common;
using Microsoft.Extensions.Caching.Memory;

namespace AspNotes.Core.Tests.Common;

public class AppCacheTests : IDisposable
{
    private readonly MemoryCache _memoryCache;
    private readonly AppCache _appCache;

    public AppCacheTests()
    {
        var options = new MemoryCacheOptions();
        _memoryCache = new MemoryCache(options);
        _appCache = new AppCache(_memoryCache);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void UpdateCache_ShouldAddItemToCache()
    {
        // Arrange
        var key = "TestKey";
        var value = "TestValue";

        // Act
        _appCache.UpdateCache(key, value);

        // Assert
        Assert.True(_memoryCache.TryGetValue(key, out var cachedValue));
        Assert.Equal(value, cachedValue);
    }

    [Fact]
    public void GetCache_ShouldRetrieveItemFromCache()
    {
        // Arrange
        var key = "TestKey";
        var value = "TestValue";
        _memoryCache.Set(key, value);

        // Act
        var cachedValue = _appCache.GetCache<string>(key);

        // Assert
        Assert.Equal(value, cachedValue);
    }

    [Fact]
    public void RemoveCache_ShouldRemoveItemFromCache()
    {
        // Arrange
        string key = "TestKey";
        string value = "TestValue";
        _memoryCache.Set(key, value);

        // Act
        _appCache.RemoveCache(key);

        // Assert
        Assert.False(_memoryCache.TryGetValue(key, out var _));
    }
}
