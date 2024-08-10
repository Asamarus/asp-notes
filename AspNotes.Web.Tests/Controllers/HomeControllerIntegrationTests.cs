using System.Net;

namespace AspNotes.Web.Tests.Controllers;
public class HomeControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    [Fact]
    public async Task Index_ReturnsCorrectTitle()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("<title>Notes</title>", responseString);
    }
}
