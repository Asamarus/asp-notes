using AspNotes.Helpers;
using AspNotes.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNotes.Tests.Middlewares;

public class CustomExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WritesProblemDetails_WhenExceptionIsThrown()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        mockRequestDelegate.Setup(rd => rd(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

        var mockLogger = new Mock<ILogger<CustomExceptionHandlerMiddleware>>();

        var mockEnv = new Mock<IHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new CustomExceptionHandlerMiddleware(mockRequestDelegate.Object, mockLogger.Object, mockEnv.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var response = await reader.ReadToEndAsync();
        var problemDetails = response.DeserializeJson<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Test exception", problemDetails.Title);
        Assert.Equal(500, problemDetails.Status);
        Assert.NotNull(problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_WritesServerErrorMessage_WhenEnvironmentIsNotDevelopment()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        mockRequestDelegate.Setup(rd => rd(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

        var mockLogger = new Mock<ILogger<CustomExceptionHandlerMiddleware>>();

        var mockEnv = new Mock<IHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Production);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new CustomExceptionHandlerMiddleware(mockRequestDelegate.Object, mockLogger.Object, mockEnv.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var response = await reader.ReadToEndAsync();
        var problemDetails = response.DeserializeJson<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Server error", problemDetails.Title);
        Assert.Equal(500, problemDetails.Status);
        Assert.Null(problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_LogsException_WhenExceptionIsThrown()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        mockRequestDelegate.Setup(rd => rd(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

        var mockLogger = new Mock<ILogger<CustomExceptionHandlerMiddleware>>();

        var mockEnv = new Mock<IHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        var context = new DefaultHttpContext();

        var middleware = new CustomExceptionHandlerMiddleware(mockRequestDelegate.Object, mockLogger.Object, mockEnv.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        mockLogger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once
       );
    }
}
