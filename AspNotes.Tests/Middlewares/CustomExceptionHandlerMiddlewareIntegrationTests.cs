using AspNotes.Interfaces;
using AspNotes.Tests.Misc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Http.Json;

namespace AspNotes.Tests.Middlewares;

public class CustomExceptionHandlerMiddlewareIntegrationTests(WebApplicationFactory<IAspNotesAssemblyMarker> factory) : IClassFixture<WebApplicationFactory<IAspNotesAssemblyMarker>>
{
    private readonly WebApplicationFactory<IAspNotesAssemblyMarker> factory = factory;

    [Fact]
    public async Task Middleware_ShouldReturnProblemDetails_WhenExceptionIsThrown()
    {
        // Arrange
        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(Environments.Development);

            builder.ConfigureTestServices(services =>
            {
                services.AddControllers().AddApplicationPart(typeof(TestControllerWithException).Assembly);
            });

        }).CreateClient();

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
        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(Environments.Production);

            builder.ConfigureTestServices(services =>
            {
                services.AddControllers().AddApplicationPart(typeof(TestControllerWithException).Assembly);
            });

        }).CreateClient();

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
