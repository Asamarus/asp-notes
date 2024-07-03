using AspNotes.Core.Common.Helpers;

namespace AspNotes.Core.Tests.Common.Helpers;

public class JsonHelperTests
{
    public class TestObject
    {
        public string Message { get; set; } = string.Empty;
    }

    [Fact]
    public void DeserializeJson_ReturnsCorrectObject_WhenJsonIsValid()
    {
        // Arrange
        string jsonString = "{\"message\":\"Test message\"}";

        // Act
        var result = jsonString.DeserializeJson<TestObject>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test message", result?.Message);
    }

    [Fact]
    public void DeserializeJson_ReturnsNull_WhenJsonIsInvalid()
    {
        // Arrange
        string jsonString = "Invalid JSON";

        // Act
        var result = jsonString.DeserializeJson<TestObject>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeJson_ReturnsNull_WhenJsonIsEmpty()
    {
        // Arrange
        string jsonString = "";

        // Act
        var result = jsonString.DeserializeJson<TestObject>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SerializeJson_ReturnsCorrectJson_WhenObjectIsValid()
    {
        // Arrange
        var testObject = new TestObject { Message = "Test message" };

        // Act
        var result = testObject.SerializeJson();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("{\"message\":\"Test message\"}", result);
    }

    [Fact]
    public void SerializeJson_ReturnsNull_WhenObjectIsNull()
    {
        // Arrange
        TestObject? testObject = null;

        // Act
        var result = testObject.SerializeJson();

        // Assert
        Assert.Null(result);
    }
}
