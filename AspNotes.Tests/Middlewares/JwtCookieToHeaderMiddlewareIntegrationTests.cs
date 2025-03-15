using AspNotes.Interfaces;
using AspNotes.Tests.Misc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AspNotes.Tests.Middlewares;
public class JwtCookieToHeaderMiddlewareIntegrationTests(WebApplicationFactory<IAspNotesAssemblyMarker> factory) : IClassFixture<WebApplicationFactory<IAspNotesAssemblyMarker>>
{
    private readonly HttpClient client = factory.WithWebHostBuilder(builder =>
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddControllers().AddApplicationPart(typeof(TestControllerWithRequestHeaderEcho).Assembly);
        });

    }).CreateClient();

    [Fact]
    public async Task Middleware_ShouldAddAuthorizationHeader_WhenJwtCookieExists()
    {
        // Arrange
        client.DefaultRequestHeaders.Add("Cookie", "AccessToken=testToken");

        // Act
        var response = await client.GetAsync("/test/echo-headers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var headers = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("Authorization"));
        Assert.Equal("Bearer testToken", headers["Authorization"]);
    }

    [Fact]
    public async Task Middleware_ShouldNotAddAuthorizationHeader_WhenJwtCookieDoesNotExist()
    {
        // Arrange

        // Act
        var response = await client.GetAsync("/test/echo-headers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var headers = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(headers);
        Assert.False(headers.ContainsKey("Authorization"));
    }
}
