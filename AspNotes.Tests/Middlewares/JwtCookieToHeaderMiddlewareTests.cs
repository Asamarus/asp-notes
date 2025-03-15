using AspNotes.Middlewares;
using AspNotes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;

namespace AspNotes.Tests.Middlewares;
public class JwtCookieToHeaderMiddlewareTests
{
    private readonly JwtSettings jwtSettings;
    private readonly Mock<IOptions<JwtSettings>> mockJwtSettings;

    public JwtCookieToHeaderMiddlewareTests()
    {
        jwtSettings = new JwtSettings
        {
            CookieName = "jwtCookie",
            Secret = "your_secret_key",
            ValidIssuer = "your_valid_issuer",
            ValidAudience = "your_valid_audience",
            AccessTokenExpirationMinutes = 60
        };
        mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);
    }

    [Fact]
    public async Task Invoke_AddsAuthorizationHeader_WhenJwtCookieExists()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var context = new DefaultHttpContext();
        context.Request.Headers.Append("Cookie", $"{jwtSettings.CookieName}=testToken");

        var middleware = new JwtCookieToHeaderMiddleware(mockRequestDelegate.Object, mockJwtSettings.Object);

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.True(context.Request.Headers.ContainsKey("Authorization"));
        Assert.Equal("Bearer testToken", context.Request.Headers["Authorization"]);
    }

    [Fact]
    public async Task Invoke_DoesNotAddAuthorizationHeader_WhenJwtCookieDoesNotExist()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var context = new DefaultHttpContext();

        var middleware = new JwtCookieToHeaderMiddleware(mockRequestDelegate.Object, mockJwtSettings.Object);

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.False(context.Request.Headers.ContainsKey("Authorization"));
    }

    [Fact]
    public async Task Invoke_DoesNotAddAuthorizationHeader_WhenJwtCookieIsEmpty()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var context = new DefaultHttpContext();
        context.Request.Headers.Append("Cookie", $"{jwtSettings.CookieName}=");

        var middleware = new JwtCookieToHeaderMiddleware(mockRequestDelegate.Object, mockJwtSettings.Object);

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.False(context.Request.Headers.ContainsKey("Authorization"));
    }

    [Fact]
    public async Task Invoke_DoesNotAddAuthorizationHeader_WhenJwtCookieNameIsDifferent()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var context = new DefaultHttpContext();
        context.Request.Headers.Append("Cookie", "differentCookie=testToken");

        var middleware = new JwtCookieToHeaderMiddleware(mockRequestDelegate.Object, mockJwtSettings.Object);

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.False(context.Request.Headers.ContainsKey("Authorization"));
    }
}
