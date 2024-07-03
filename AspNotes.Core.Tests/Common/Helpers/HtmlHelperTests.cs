using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Common.Models;

namespace AspNotes.Core.Tests.Common.Helpers;

public class HtmlHelperTests
{
    [Fact]
    public void StripTags_ShouldReturnEmptyString_WhenInputIsEmpty()
    {
        // Arrange
        var input = "";

        // Act
        var result = HtmlHelper.StripTags(input);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void StripTags_ShouldReturnTextWithoutHtmlTags_WhenInputIsHtml()
    {
        // Arrange
        var input = "<p>Hello, World!</p>";

        // Act
        var result = HtmlHelper.StripTags(input);

        // Assert
        Assert.Equal("Hello, World!", result);
    }

    [Fact]
    public void ParseHtmlForMetadata_ShouldReturnDefaultMetadata_WhenInputIsEmpty()
    {
        // Arrange
        var input = "";
        var expected = new HtmlDocumentMetadata();

        // Act
        var result = HtmlHelper.ParseHtmlForMetadata(input);

        // Assert
        Assert.Equal(expected.Title, result.Title);
        Assert.Equal(expected.Description, result.Description);
        Assert.Equal(expected.Image, result.Image);
    }

    [Fact]
    public void ParseHtmlForMetadata_ShouldReturnCorrectMetadata_WhenInputIsHtmlWithOgTags()
    {
        // Arrange
        var html = @"
        <html>
        <head>
            <title>Test Title</title>
            <meta name='description' content='Test Description'>
            <meta property='og:title' content='OG Title'>
            <meta property='og:description' content='OG Description'>
            <meta property='og:image' content='OG Image'>
        </head>
        <body>
        </body>
        </html>";

        // Act
        var result = HtmlHelper.ParseHtmlForMetadata(html);

        // Assert
        Assert.Equal("OG Title", result.Title);
        Assert.Equal("OG Description", result.Description);
        Assert.Equal("OG Image", result.Image);
    }

    [Fact]
    public void ParseHtmlForMetadata_ShouldReturnCorrectMetadata_WhenInputIsHtmlWithoutOgTags()
    {
        // Arrange
        var html = @"
        <html>
        <head>
            <title>Test Title</title>
            <meta name='description' content='Test Description'>
            <meta property='og:image' content='OG Image'>
        </head>
        <body>
        </body>
        </html>";

        // Act
        var result = HtmlHelper.ParseHtmlForMetadata(html);

        // Assert
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("OG Image", result.Image);
    }
}
