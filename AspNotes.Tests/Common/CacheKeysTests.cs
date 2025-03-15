using AspNotes.Common;

namespace AspNotes.Tests.Common;

public class CacheKeysTests
{
    [Fact]
    public void Sections_ShouldHaveExpectedValue()
    {
        // Arrange
        var expectedValue = "Sections";

        // Act
        var actualValue = CacheKeys.Sections;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }
}
