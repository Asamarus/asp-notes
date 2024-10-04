using AspNotes.Core.Common.Helpers;
using AspNotes.Web.Helpers;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Notes;
using AspNotes.Web.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AspNotes.Web.Tests.Controllers.Api;

public class NotesControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    //[Fact]
    //public async Task SearchNotes_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = SearchNotesRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/search", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<SearchNotesResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = SearchNotesResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task AutocompleteNotes_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = AutocompleteNotesRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/autocomplete", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<AutocompleteNotesResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = AutocompleteNotesResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task GetNote_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = GetNoteRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/get", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<GetNoteResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = GetNoteResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task CreateNote_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = CreateNoteRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/create", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<CreateNoteResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = CreateNoteResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task UpdateNote_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = UpdateNoteRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/update", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<UpdateNoteResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = UpdateNoteResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task UpdateNoteBook_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = UpdateNoteBookRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/updateBook", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<UpdateNoteBookResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = UpdateNoteBookResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task UpdateNoteTags_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = UpdateNoteTagsRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/updateTags", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<UpdateNoteTagsResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = UpdateNoteTagsResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task UpdateNoteSection_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = UpdateNoteSectionRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/updateSection", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<UpdateNoteSectionResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = UpdateNoteSectionResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task DeleteNote_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = DeleteNoteRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/delete", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<DeleteNoteResponse>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = DeleteNoteResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}

    //[Fact]
    //public async Task GetCalendarDays_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    var requestData = GetNoteCalendarDaysRequestMock.Get();
    //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //    // Act
    //    var response = await client.PostAsync("/api/notes/getCalendarDays", requestContent);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<NoteCalendarDaysResponseItem>();

    //    Assert.NotNull(responseData);
    //    var responseMockData = GetNoteCalendarDaysResponseMock.Get();
    //    Assert.Equal(responseMockData.Data, responseData.Data);
    //}
}
