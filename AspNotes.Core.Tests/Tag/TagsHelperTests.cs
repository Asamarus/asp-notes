using AspNotes.Core.Tag;

namespace AspNotes.Core.Tests.Tag;

public class TagsHelperTests
{
    [Fact]
    public void Compress_ReturnsCorrectString_WhenListIsValid()
    {
        // Arrange
        var tags = new List<string> { "tag1", "tag2", "tag3" };

        // Act
        var result = TagsHelper.Compress(tags);

        // Assert
        Assert.Equal("[tag1],[tag2],[tag3]", result);
    }

    [Fact]
    public void Compress_ReturnsEmptyString_WhenListIsEmpty()
    {
        // Arrange
        var tags = new List<string>();

        // Act
        var result = TagsHelper.Compress(tags);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void Extract_ReturnsCorrectList_WhenStringIsValid()
    {
        // Arrange
        string tags = "[tag1],[tag2],[tag3]";

        // Act
        var result = TagsHelper.Extract(tags);

        // Assert
        Assert.Equal(["tag1", "tag2", "tag3"], result);
    }

    [Fact]
    public void Extract_ReturnsEmptyList_WhenStringIsEmpty()
    {
        // Arrange
        string tags = "";

        // Act
        var result = TagsHelper.Extract(tags);

        // Assert
        Assert.Equal([], result);
    }
}
