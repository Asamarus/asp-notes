using AspNotes.Helpers;

namespace AspNotes.Tests.Helpers;

public class JsonHelperTests
{
    private class TestClass
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    [Fact]
    public void DeserializeJson_ShouldReturnObjectForValidJson()
    {
        // Arrange
        var json = "{\"name\":\"John\",\"age\":30}";

        // Act
        var result = json.DeserializeJson<TestClass>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result?.Name);
        Assert.Equal(30, result?.Age);
    }

    [Fact]
    public void DeserializeJson_ShouldReturnDefaultForInvalidJson()
    {
        // Arrange
        var json = "invalid json";

        // Act
        var result = json.DeserializeJson<TestClass>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeJson_ShouldReturnDefaultForNullOrWhitespaceJson()
    {
        // Act & Assert
        Assert.Null(((string?)null).DeserializeJson<TestClass>());
        Assert.Null("".DeserializeJson<TestClass>());
        Assert.Null("   ".DeserializeJson<TestClass>());
    }

    [Fact]
    public void SerializeJson_ShouldReturnJsonStringForValidObject()
    {
        // Arrange
        var obj = new TestClass { Name = "John", Age = 30 };

        // Act
        var result = obj.SerializeJson();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("\"name\":\"John\"", result);
        Assert.Contains("\"age\":30", result);
    }

    [Fact]
    public void SerializeJson_ShouldReturnNullForNullObject()
    {
        // Act
        var result = ((TestClass?)null).SerializeJson();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetJsonConverter_ShouldConvertObjectToJsonAndBack()
    {
        // Arrange
        var converter = JsonHelper.GetJsonConverter<TestClass>();
        var obj = new TestClass { Name = "John", Age = 30 };

        // Act
        var json = converter.ConvertToProvider(obj);
        var result = converter.ConvertFromProvider(json) as TestClass;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
        Assert.Equal(30, result.Age);
    }
}
