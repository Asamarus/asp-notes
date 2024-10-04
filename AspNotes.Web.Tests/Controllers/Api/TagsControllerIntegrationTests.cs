using AspNotes.Core.Common.Helpers;
using AspNotes.Web.Helpers;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Tags;
using AspNotes.Web.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AspNotes.Web.Tests.Controllers.Api;

public class TagsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    //[Fact]
    //public async Task GetTagsList_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = GetTagsListRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/tags/getList", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<GetTagsListResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = GetTagsListResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task AutocompleteTags_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = AutocompleteTagsRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/tags/autocomplete", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<AutocompleteTagsResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = AutocompleteTagsResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}
}