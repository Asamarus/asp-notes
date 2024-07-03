using AspNotes.Web.Middlewares;
using AspNotes.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;

namespace AspNotes.Web.Tests.Middlewares;

public class JwtCookieToHeaderMiddlewareTests
{
    [Fact]
    public async Task Invoke_AddsAuthorizationHeader_WhenJwtCookieExists()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var jwtSettings = new JwtSettings { CookieName = "jwtCookie" };
        var mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);
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
        var jwtSettings = new JwtSettings { CookieName = "jwtCookie" };
        var mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);
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
        var jwtSettings = new JwtSettings { CookieName = "jwtCookie" };
        var mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);
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
        var jwtSettings = new JwtSettings { CookieName = "jwtCookie" };
        var mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);
        var context = new DefaultHttpContext();
        context.Request.Headers.Append("Cookie", "differentCookie=testToken");

        var middleware = new JwtCookieToHeaderMiddleware(mockRequestDelegate.Object, mockJwtSettings.Object);

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.False(context.Request.Headers.ContainsKey("Authorization"));
    }
}
