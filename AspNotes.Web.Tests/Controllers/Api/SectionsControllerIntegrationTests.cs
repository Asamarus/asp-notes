using AspNotes.Core.Common;
using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Section.Models;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Sections;
using AspNotes.Web.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Web.Tests.Controllers.Api;

public class SectionsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
{
    public void Dispose()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        TestHelper.ClearDatabase(dbContext);
        TestHelper.ClearCache(cache);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetSectionsList_ReturnsOk()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();

        var testSections = new List<SectionEntity>
        {
            new() { Name = "TestSection1", DisplayName = "Test Section 1", Color = "#FF0000", Position = 1 },
            new() { Name = "TestSection2", DisplayName = "Test Section 2", Color = "#00FF00", Position = 2 }
        };

        dbContext.Sections.AddRange(testSections);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsync("/api/sections/getList", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = responseContent.DeserializeJson<SectionsResponse>();

        Assert.NotNull(responseData);
        Assert.NotNull(responseData.Sections);
        Assert.Equal(testSections.Count, responseData.Sections.Count);

        foreach (var testSection in testSections)
        {
            var responseSection = responseData.Sections.FirstOrDefault(s => s.Name == testSection.Name);
            Assert.NotNull(responseSection);
            Assert.Equal(testSection.DisplayName, responseSection.DisplayName);
            Assert.Equal(testSection.Color, responseSection.Color);
        }
    }

    [Fact]
    public async Task CreateSection_ReturnsBadRequest_WithInvalidData()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var updateRequest = new CreateSectionRequest
        {
            Name = "Invalid",
            DisplayName = "",
            Color = "Red"
        };
        var content = JsonContent.Create(updateRequest);

        // Act
        var response = await client.PostAsync("/api/sections/create", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var validationErrors = responseString.DeserializeJson<ProblemDetails>();

        Assert.NotNull(validationErrors);
        Assert.Equal(3, validationErrors.Errors.Count);

        var idError = validationErrors.Errors.FirstOrDefault(e => e.Key == "Name");
        Assert.Contains("Name must contain only Latin letters and no whitespaces!", idError.Value);

        var colorError = validationErrors.Errors.FirstOrDefault(e => e.Key == "Color");
        Assert.Contains("Color must be a valid #hex color!", colorError.Value);

        var displayNameError = validationErrors.Errors.FirstOrDefault(e => e.Key == "DisplayName");
        Assert.Contains("The DisplayName field is required.", displayNameError.Value);
    }

    [Fact]
    public async Task CreateSection_ReturnsOk_WithValidData()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();

        var newSection = new CreateSectionRequest
        {
            Name = "newsection",
            DisplayName = "New Section",
            Color = "#000000"
        };
        var content = JsonContent.Create(newSection);

        // Act
        var response = await client.PostAsync("/api/sections/create", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = responseContent.DeserializeJson<SectionsResponse>();

        Assert.NotNull(responseData);

        Assert.Equal("Section is created successfully!", responseData.Message);

        Assert.NotNull(responseData.Sections);

        var createdSection = responseData.Sections.FirstOrDefault(s => s.Name == newSection.Name);

        Assert.NotNull(createdSection);

        Assert.True(createdSection.Id > 0);
        Assert.Equal(newSection.Name, createdSection.Name);
        Assert.Equal(newSection.DisplayName, createdSection.DisplayName);
        Assert.Equal(newSection.Color, createdSection.Color);
    }

    [Fact]
    public async Task UpdateSection_ReturnsBadRequest_WithInvalidData()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var updateRequest = new UpdateSectionRequest
        {
            Id = -1,
            DisplayName = "",
            Color = "Red"
        };
        var content = JsonContent.Create(updateRequest);

        // Act
        var response = await client.PostAsync("/api/sections/update", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var validationErrors = responseString.DeserializeJson<ProblemDetails>();

        Assert.NotNull(validationErrors);
        Assert.Equal(3, validationErrors.Errors.Count);

        var idError = validationErrors.Errors.FirstOrDefault(e => e.Key == "Id");
        Assert.Contains("Id must be a positive number!", idError.Value);

        var colorError = validationErrors.Errors.FirstOrDefault(e => e.Key == "Color");
        Assert.Contains("Color must be a valid #hex color!", colorError.Value);

        var displayNameError = validationErrors.Errors.FirstOrDefault(e => e.Key == "DisplayName");
        Assert.Contains("The DisplayName field is required.", displayNameError.Value);
    }

    [Fact]
    public async Task UpdateSection_ReturnsOk_WithValidData()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
        var testSection = new SectionEntity { Name = "OriginalName", DisplayName = "Original Display Name", Color = "#FFFFFF", Position = 1 };
        dbContext.Sections.Add(testSection);
        await dbContext.SaveChangesAsync();

        var updateRequest = new UpdateSectionRequest
        {
            Id = testSection.Id,
            DisplayName = "Updated Display Name",
            Color = "#000000"
        };
        var content = JsonContent.Create(updateRequest);

        // Act
        var response = await client.PostAsync("/api/sections/update", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = responseContent.DeserializeJson<SectionsResponse>();

        Assert.NotNull(responseData);
        Assert.Contains(responseData.Sections, s => s.Id == testSection.Id && s.DisplayName == "Updated Display Name" && s.Color == "#000000");
    }

    [Fact]
    public async Task DeleteSection_ReturnsOk_WhenSectionExists()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();

        var testSection = new SectionEntity { Name = "SectionToDelete", DisplayName = "Section To Delete", Color = "#FF0000", Position = 1 };
        dbContext.Sections.Add(testSection);
        await dbContext.SaveChangesAsync();

        var deleteRequest = new DeleteSectionRequest
        {
            Id = testSection.Id,
        };
        var content = JsonContent.Create(deleteRequest);

        // Act
        var response = await client.PostAsync("/api/sections/delete", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deletedSection = dbContext.Sections.FirstOrDefault(s => s.Id == testSection.Id);
        Assert.Null(deletedSection);
    }

    [Fact]
    public async Task DeleteSection_ReturnsBadRequest_WhenSectionDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var deleteRequest = new DeleteSectionRequest
        {
            Id = 999,
        };
        var content = JsonContent.Create(deleteRequest);

        // Act
        var response = await client.PostAsync("/api/sections/delete", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var errorResponse = responseString.DeserializeJson<ErrorResponse>();

        Assert.NotNull(errorResponse);
        Assert.Equal("Section with Id '999' is not found!", errorResponse.Message);
    }

    [Fact]
    public async Task DeleteSection_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var deleteRequest = new DeleteSectionRequest
        {
            Id = -1,
        };
        var content = JsonContent.Create(deleteRequest);

        // Act
        var response = await client.PostAsync("/api/sections/delete", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var validationErrors = responseString.DeserializeJson<ProblemDetails>();

        Assert.NotNull(validationErrors);
        Assert.Single(validationErrors.Errors);

        var idError = validationErrors.Errors.FirstOrDefault(e => e.Key == "Id");
        Assert.Contains("Id must be a positive number!", idError.Value);
    }

    [Fact]
    public async Task ReorderSections_ReturnsOk_WhenValidRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();

        var testSections = new List<SectionEntity>
        {
            new() { Name = "TestSection1", DisplayName = "Test Section 1", Color = "#FF0000", Position = 2 },
            new() { Name = "TestSection2", DisplayName = "Test Section 2", Color = "#00FF00", Position = 1 }
        };

        dbContext.Sections.AddRange(testSections);
        await dbContext.SaveChangesAsync();

        var sectionIds = testSections.Select(s => s.Id).Reverse().ToList();

        var reorderRequest = new ReorderSectionsRequest
        {
            Ids = sectionIds
        };
        var content = JsonContent.Create(reorderRequest);

        // Act
        var response = await client.PostAsync("/api/sections/reorder", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var reorderedSections = dbContext.Sections.OrderBy(s => s.Position).ToList();
        Assert.Equal(testSections.Count, reorderedSections.Count);

        Assert.Equal(sectionIds, reorderedSections.Select(s => s.Id).ToList());
    }

    [Fact]
    public async Task ReorderSections_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var reorderRequest = new ReorderSectionsRequest
        {
            Ids = [1, 2, 3]
        };
        var content = JsonContent.Create(reorderRequest);

        // Act
        var response = await client.PostAsync("/api/sections/reorder", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var errorResponse = responseString.DeserializeJson<ErrorResponse>();

        Assert.NotNull(errorResponse);
        Assert.Equal("Sections are not reordered!", errorResponse.Message);
    }

    [Fact]
    public async Task ReorderSections_ReturnsBadRequest_WhenRequestIdsAreNotPositiveNumbers()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var reorderRequest = new ReorderSectionsRequest
        {
            Ids = [-1, 0, 1]
        };
        var content = JsonContent.Create(reorderRequest);

        // Act
        var response = await client.PostAsync("/api/sections/reorder", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var validationErrors = responseString.DeserializeJson<ProblemDetails>();

        Assert.NotNull(validationErrors);
        Assert.Single(validationErrors.Errors);

        var idError = validationErrors.Errors.FirstOrDefault(e => e.Key == "Ids");
        Assert.Contains("All Ids must be positive numbers!", idError.Value);
    }
}