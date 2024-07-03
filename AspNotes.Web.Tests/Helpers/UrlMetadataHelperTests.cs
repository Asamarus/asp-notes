using AspNotes.Web.Helpers;
using Moq;
using Moq.Protected;
using System.Net;

namespace AspNotes.Web.Tests.Helpers;

public class UrlMetadataHelperTests
{
    [Fact]
    public async Task GetUrlMetadata_ReturnsCorrectMetadata()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(@"<html>
                                                <head>
                                                    <meta property=""og:title"" content=""Test Title"" />
                                                    <meta property=""og:description"" content=""Test Description"" />
                                                    <meta property=""og:image"" content=""Test Image URL"" />
                                                </head>
                                            </html>"),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var urlHelper = new UrlMetadataHelper(httpClientFactoryMock.Object);

        // Act
        var metadata = await urlHelper.GetUrlMetadata("http://test.com");

        // Assert
        Assert.Equal("Test Title", metadata.Title);
        Assert.Equal("Test Description", metadata.Description);
        Assert.Equal("Test Image URL", metadata.Image);

        handlerMock.Protected().Verify(
           "SendAsync",
           Times.Exactly(1), // we expected a single external request
           ItExpr.IsAny<HttpRequestMessage>(),
           ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetUrlMetadata_ReturnsFallbackMetadata()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(@"<html>
                                            <head>
                                                <title>Fallback Title</title>
                                                <meta name=""description"" content=""Fallback Description"" />
                                            </head>
                                        </html>"),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var urlHelper = new UrlMetadataHelper(httpClientFactoryMock.Object);

        // Act
        var metadata = await urlHelper.GetUrlMetadata("http://test.com");

        // Assert
        Assert.Equal("Fallback Title", metadata.Title);
        Assert.Equal("Fallback Description", metadata.Description);

        handlerMock.Protected().Verify(
           "SendAsync",
           Times.Exactly(1), // we expected a single external request
           ItExpr.IsAny<HttpRequestMessage>(),
           ItExpr.IsAny<CancellationToken>());
    }
}
