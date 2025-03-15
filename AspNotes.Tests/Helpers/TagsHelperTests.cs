using AspNotes.Helpers;

namespace AspNotes.Tests.Helpers;

public class TagsHelperTests
{
    [Fact]
    public void Compress_ShouldReturnCompressedString()
    {
        // Arrange
        var tags = new List<string> { "tag1", "tag2", "tag3" };

        // Act
        var result = TagsHelper.Compress(tags);

        // Assert
        Assert.Equal("[tag1],[tag2],[tag3]", result);
    }

    [Fact]
    public void Compress_ShouldReturnCompressedStringInAlphabeticalOrder()
    {
        // Arrange
        var tags = new List<string> { "tag3", "tag1", "tag2" };

        // Act
        var result = TagsHelper.Compress(tags);

        // Assert
        Assert.Equal("[tag1],[tag2],[tag3]", result);
    }

    [Fact]
    public void Extract_ShouldReturnListOfTags()
    {
        // Arrange
        var compressedTags = "[tag1],[tag2],[tag3]";

        // Act
        var result = TagsHelper.Extract(compressedTags);

        // Assert
        Assert.Equal(["tag1", "tag2", "tag3"], result);
    }

    [Fact]
    public void Extract_ShouldReturnEmptyListForEmptyString()
    {
        // Act
        var result = TagsHelper.Extract(string.Empty);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetTagsConverter_ShouldConvertListToStringAndBack()
    {
        // Arrange
        var converter = TagsHelper.GetTagsConverter();
        var tags = new List<string> { "tag1", "tag2", "tag3" };

        // Act
        var compressedTags = converter.ConvertToProvider(tags);
        var result = converter.ConvertFromProvider(compressedTags);

        // Assert
        Assert.Equal("[tag1],[tag2],[tag3]", compressedTags);
        Assert.Equal(tags, result);
    }
}
