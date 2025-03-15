using AspNotes.Common.Converters;
using System.Text.Json;

namespace AspNotes.Tests.Common.Converters;

public class TrimmingStringJsonConverterTests
{
    private readonly JsonSerializerOptions options;

    public TrimmingStringJsonConverterTests()
    {
        options = new JsonSerializerOptions
        {
            Converters = { new TrimmingStringJsonConverter() }
        };
    }

    [Fact]
    public void Read_TrimsLeadingAndTrailingWhitespace()
    {
        // Arrange
        var json = "\"  test string  \"";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, options);

        // Assert
        Assert.Equal("test string", result);
    }

    [Fact]
    public void Read_NullValue_ReturnsNull()
    {
        // Arrange
        var json = "null";

        // Act
        var result = JsonSerializer.Deserialize<string>(json, options);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Write_WritesStringValueWithoutModification()
    {
        // Arrange
        var value = "test string";
        var expectedJson = "\"test string\"";

        // Act
        var json = JsonSerializer.Serialize(value, options);

        // Assert
        Assert.Equal(expectedJson, json);
    }
}
