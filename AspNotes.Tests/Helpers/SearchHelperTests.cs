using AspNotes.Helpers;

namespace AspNotes.Tests.Helpers;

public class SearchHelperTests
{
    [Fact]
    public void GetSearchIndex_ShouldReturnLowerCaseIndex()
    {
        // Arrange
        var content = "<p>Hello</p><p>World</p>";

        // Act
        var result = SearchHelper.GetSearchIndex(content);

        // Assert
        Assert.Equal("hello world", result);
    }

    [Fact]
    public void GetSearchIndex_ShouldReturnLimitedLengthIndex()
    {
        // Arrange
        var content = "<p>Hello</p><p>World</p>";

        // Act
        var result = SearchHelper.GetSearchIndex(content, limit: 5);

        // Assert
        Assert.Equal("hello...", result);
    }

    [Fact]
    public void GetSearchIndex_ShouldReturnEmptyStringForEmptyContent()
    {
        // Act
        var result = SearchHelper.GetSearchIndex(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetSearchIndex_ShouldSeparateWordsInDifferentHtmlTagsWithSpace()
    {
        // Arrange
        var content = "<p>Hello</p><p>world</p>";
        var expected = "Hello world";

        // Act
        var result = SearchHelper.GetSearchIndex(content, false);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnHighlightedSnippet()
    {
        // Arrange
        var keywords = new HashSet<string> { "world" };
        var text = "Hello world! This is a test world.";

        // Act
        var result = SearchHelper.GetSearchSnippet(keywords, text, highlight: true);

        // Assert
        Assert.Contains("<em>world</em>", result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnLimitedSnippets()
    {
        // Arrange
        var keywords = new HashSet<string> { "world" };
        var text = "Hello world! This is a test world. Another world here.";

        // Act
        var result = SearchHelper.GetSearchSnippet(keywords, text, limit: 1);

        // Assert
        Assert.Equal("Hello world! This is a tes...", result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnEmptyStringForNoKeywords()
    {
        // Act
        var result = SearchHelper.GetSearchSnippet([], "Some text");

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnEmptyStringForEmptyText()
    {
        // Act
        var result = SearchHelper.GetSearchSnippet(["keyword"], string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnCorrectSnippet_WithHighlightedKeywords()
    {
        // Arrange
        var keywords = new HashSet<string> { "lorem", "ipsum", "dolor" };
        var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed non risus. Suspendisse lectus tortor, dignissim sit amet, adipiscing nec, ultricies sed, dolor. Cras elementum ultrices diam. Maecenas ligula massa, varius a, semper congue, euismod non, mi.";
        var highlight = true;

        // Act
        var result = SearchHelper.GetSearchSnippet(keywords, text, highlight: highlight);

        // Assert
        Assert.Contains("<em>Lorem</em> ipsum dolor si...", result);
        Assert.Contains("Lorem <em>ipsum</em> dolor sit amet...", result);
        Assert.Contains("Lorem ipsum <em>dolor</em> sit amet, cons...", result);
        Assert.Contains("ultricies sed, <em>dolor</em>. Cras elementu...", result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldHighlightWholePhrase()
    {
        // Arrange
        var keywords = new HashSet<string> { "hello world" };
        var text = "This is a test for Hello World highlighting.";
        var expectedSnippet = "is a test for <em>Hello World</em> highlighting....";

        // Act
        var snippet = SearchHelper.GetSearchSnippet(keywords, text, foundWholePhrase: true, highlight: true);

        // Assert
        Assert.Equal(expectedSnippet, snippet);
    }
}
