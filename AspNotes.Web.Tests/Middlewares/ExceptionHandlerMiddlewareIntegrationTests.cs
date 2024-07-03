using AspNotes.Web.Models.Common;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;

namespace AspNotes.Web.Tests.Middlewares;
public class ExceptionHandlerMiddlewareIntegrationTests
{

    [Fact]
    public async Task InvokeAsync_ReturnsProblemDetails_WhenExceptionIsThrown()
    {
        // Arrange
        using var factory = new WebApplicationFactoryWithException<Startup>();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/test");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Test exception", problemDetails.Title);
        Assert.Equal(500, problemDetails.Status);
        Assert.NotNull(problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsServerErrorMessage_WhenEnvironmentIsNotDevelopment()
    {
        // Arrange
        using var factory = new ProductionWebApplicationFactoryWithException<Startup>();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/test");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Server error", problemDetails.Title);
        Assert.Equal(500, problemDetails.Status);
        Assert.Null(problemDetails.Detail);
    }
}
