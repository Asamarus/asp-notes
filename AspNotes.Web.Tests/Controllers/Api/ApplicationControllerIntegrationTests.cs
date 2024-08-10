using AspNotes.Core.Common.Helpers;
using AspNotes.Web.Models.Application;
using AspNotes.Web.Tests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;

namespace AspNotes.Web.Tests.Controllers.Api;
public class ApplicationControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    [Fact]
    public async Task GetInitialData_ReturnsInitialDataResponse()
    {
        // Arrange
        var allNotesSection = TestHelper.GetAllNotesSection();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        // Act
        var response = await client.PostAsync("/api/application/getInitialData", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);

        var responseString = await response.Content.ReadAsStringAsync();
        var initialDataResponse = responseString.DeserializeJson<InitialDataResponse>();

        Assert.NotNull(initialDataResponse);
        initialDataResponse.AllNotesSection.Should().BeEquivalentTo(allNotesSection);
    }
}


