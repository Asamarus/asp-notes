using AspNotes.Helpers;

namespace AspNotes.Tests.Helpers;

public class HtmlHelperTests
{
    [Fact]
    public void StripTags_ShouldReturnTextWithoutHtmlTags()
    {
        // Arrange
        var html = "<p>This is <strong>bold</strong> and <em>italic</em> text.</p>";

        // Act
        var result = HtmlHelper.StripTags(html);

        // Assert
        Assert.Equal("This is bold and italic text.", result);
    }

    [Fact]
    public void StripTags_ShouldReturnEmptyStringForNullOrEmptyInput()
    {
        // Act & Assert
        Assert.Equal(string.Empty, HtmlHelper.StripTags(null));
        Assert.Equal(string.Empty, HtmlHelper.StripTags(string.Empty));
    }

    [Fact]
    public void ParseHtmlForMetadata_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var html = @"
                <html>
                    <head>
                        <meta property='og:title' content='Test Title' />
                        <meta property='og:description' content='Test Description' />
                        <meta property='og:image' content='test-image.jpg' />
                        <title>Fallback Title</title>
                        <meta name='description' content='Fallback Description' />
                    </head>
                    <body></body>
                </html>";

        // Act
        var metadata = HtmlHelper.ParseHtmlForMetadata(html);

        // Assert
        Assert.Equal("Test Title", metadata.Title);
        Assert.Equal("Test Description", metadata.Description);
        Assert.Equal("test-image.jpg", metadata.Image);
    }

    [Fact]
    public void ParseHtmlForMetadata_ShouldUseFallbackTitleAndDescription()
    {
        // Arrange
        var html = @"
                <html>
                    <head>
                        <title>Fallback Title</title>
                        <meta name='description' content='Fallback Description' />
                    </head>
                    <body></body>
                </html>";

        // Act
        var metadata = HtmlHelper.ParseHtmlForMetadata(html);

        // Assert
        Assert.Equal("Fallback Title", metadata.Title);
        Assert.Equal("Fallback Description", metadata.Description);
        Assert.Null(metadata.Image);
    }

    [Fact]
    public void ParseHtmlForMetadata_ShouldHandleMissingMetadata()
    {
        // Arrange
        var html = "<html><head></head><body></body></html>";

        // Act
        var metadata = HtmlHelper.ParseHtmlForMetadata(html);

        // Assert
        Assert.Null(metadata.Title);
        Assert.Null(metadata.Description);
        Assert.Null(metadata.Image);
    }
}
