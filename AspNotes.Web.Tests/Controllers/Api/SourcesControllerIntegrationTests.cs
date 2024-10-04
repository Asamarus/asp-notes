using AspNotes.Core.Common.Helpers;
using AspNotes.Web.Helpers;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Sources;
using AspNotes.Web.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AspNotes.Web.Tests.Controllers.Api;

public class SourcesControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    //[Fact]
    //public async Task AddNoteSource_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = AddNoteSourceRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/sources/add", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<AddNoteSourceResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = AddNoteSourceResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task UpdateNoteSource_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = UpdateNoteSourceRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/sources/update", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<UpdateNoteSourceResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = UpdateNoteSourceResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task RemoveNoteSource_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = RemoveNoteSourceRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/sources/remove", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<RemoveNoteSourceResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = RemoveNoteSourceResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task ReorderNoteSources_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = ReorderNoteSourcesRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/sources/reorder", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<ReorderNoteSourcesResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = ReorderNoteSourcesResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}
}