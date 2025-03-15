using AspNotes.Helpers;
using Moq.Protected;
using Moq;
using System.Net;

namespace AspNotes.Tests.Helpers;

public class UrlMetadataHelperTests
{
    [Fact]
    public async Task GetUrlMetadata_ShouldReturnMetadataForValidUrl()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"<html>
                                                <head>
                                                    <meta property=""og:title"" content=""Test Title"" />
                                                    <meta property=""og:description"" content=""Test Description"" />
                                                    <meta property=""og:image"" content=""Test Image URL"" />
                                                </head>
                                            </html>")
            });

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(mockHttpMessageHandler.Object));

        var urlMetadataHelper = new UrlMetadataHelper(mockHttpClientFactory.Object);

        // Act
        var metadata = await urlMetadataHelper.GetUrlMetadata("http://example.com", CancellationToken.None);

        // Assert
        Assert.NotNull(metadata);
        Assert.Equal("Test Title", metadata.Title);
        Assert.Equal("Test Description", metadata.Description);
        Assert.Equal("Test Image URL", metadata.Image);
    }

    [Fact]
    public async Task GetUrlMetadata_ShouldReturnEmptyMetadataForInvalidUrl()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(mockHttpMessageHandler.Object));

        var urlMetadataHelper = new UrlMetadataHelper(mockHttpClientFactory.Object);

        // Act
        var metadata = await urlMetadataHelper.GetUrlMetadata("http://invalid-url.com", CancellationToken.None);

        // Assert
        Assert.NotNull(metadata);
        Assert.Null(metadata.Title);
        Assert.Null(metadata.Description);
        Assert.Null(metadata.Image);
    }
}
