using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Tests.Controllers;

public class SectionsControllerIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    private readonly ApiFactory factory;

    public SectionsControllerIntegrationTests(ApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());
    }

    [Fact]
    public async Task Get_ReturnsAllSections_WhenCalled()
    {
        // Arrange
        await SeedTestSectionsDataAsync();

        // Act
        var response = await client.GetAsync("/api/sections");

        // Assert
        response.EnsureSuccessStatusCode();
        var sections = await response.Content.ReadFromJsonAsync<List<SectionsItemResponse>>();

        Assert.NotNull(sections);
        Assert.NotEmpty(sections);
        Assert.Equal(2, sections.Count);
        Assert.Contains(sections, s => s.Name == "TestSection1");
        Assert.Contains(sections, s => s.Name == "TestSection2");
    }

    [Fact]
    public async Task Post_CreatesNewSection_WhenRequestIsValid()
    {
        // Arrange
        var request = new SectionsCreateRequest
        {
            Name = "new_test_section",
            DisplayName = "New Test Section",
            Color = "#ffffff"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/sections", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var sections = await response.Content.ReadFromJsonAsync<List<SectionsItemResponse>>();

        Assert.NotNull(sections);
        Assert.Contains(sections, s => s.Name == "new_test_section" && s.DisplayName == "New Test Section" && s.Color == "#ffffff");
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenNameIsNotUnique()
    {
        // Arrange
        await SeedTestSectionsDataAsync();

        var request = new SectionsCreateRequest
        {
            Name = "TestSection1", // Already exists
            DisplayName = "Duplicate Section",
            Color = "#123456"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/sections", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesSection_WhenRequestIsValid()
    {
        // Arrange
        var sectionId = await SeedAndGetTestSectionId("UpdateSection", "Original Name", "#000000");

        var request = new SectionsUpdateRequest
        {
            DisplayName = "Updated Name",
            Color = "#FFFFFF"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/sections/{sectionId}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var sections = await response.Content.ReadFromJsonAsync<List<SectionsItemResponse>>();

        Assert.NotNull(sections);
        var updatedSection = sections.FirstOrDefault(s => s.Id == sectionId);
        Assert.NotNull(updatedSection);
        Assert.Equal("UpdateSection", updatedSection.Name);
        Assert.Equal("Updated Name", updatedSection.DisplayName);
        Assert.Equal("#FFFFFF", updatedSection.Color);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenSectionIdDoesNotExist()
    {
        // Arrange
        var request = new SectionsUpdateRequest
        {
            DisplayName = "Updated Name",
            Color = "#FFFFFF"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/sections/999999", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var sectionId = await SeedAndGetTestSectionId("UpdateSection", "Original Name", "#000000");

        var request = new SectionsUpdateRequest
        {
            DisplayName = "",
            Color = "test"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/sections/{sectionId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Contains("DisplayName", problemDetails.Errors.Keys);
        Assert.Contains("Color", problemDetails.Errors.Keys);
    }

    [Fact]
    public async Task Delete_RemovesSection_WhenSectionExistsWithNoNotes()
    {
        // Arrange
        var sectionId = await SeedAndGetTestSectionId("DeleteSection", "Delete Test", "#FF0000");

        // Act
        var response = await client.DeleteAsync($"/api/sections/{sectionId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var sections = await response.Content.ReadFromJsonAsync<List<SectionsItemResponse>>();

        Assert.NotNull(sections);
        Assert.DoesNotContain(sections, s => s.Id == sectionId);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenSectionIdDoesNotExist()
    {
        // Act
        var response = await client.DeleteAsync("/api/sections/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenSectionHasNotes()
    {
        // Arrange
        var sectionId = await SeedTestSectionWithNotes();

        // Act
        var response = await client.DeleteAsync($"/api/sections/{sectionId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Reorder_UpdatesSectionPositions_WhenRequestIsValid()
    {
        // Arrange
        await SeedTestSectionsDataAsync();
        var sections = await GetAllSections();
        var sectionIds = sections.Select(s => s.Id).ToList();

        // Reverse the order
        sectionIds.Reverse();

        var request = new SectionsReorderRequest
        {
            Ids = sectionIds
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/sections/reorder", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedSections = await response.Content.ReadFromJsonAsync<List<SectionsItemResponse>>();

        Assert.NotNull(updatedSections);

        // Check if the order has been reversed
        for (int i = 0; i < sectionIds.Count; i++)
        {
            Assert.Equal(sectionIds[i], updatedSections[i].Id);
        }
    }

    [Fact]
    public async Task Reorder_ReturnsBadRequest_WhenIdsAreInvalid()
    {
        // Arrange
        var request = new SectionsReorderRequest
        {
            Ids = [999999, 888888] // Non-existent IDs
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/sections/reorder", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task SeedTestSectionsDataAsync()
    {
        using var scope = factory.Services.CreateScope();        
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        dbContext.Sections.RemoveRange(await dbContext.Sections.ToListAsync());
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();

        await sectionsService.CreateSection("TestSection1", "Test Section 1", "#111111", CancellationToken.None);
        await sectionsService.CreateSection("TestSection2", "Test Section 2", "#222222", CancellationToken.None);
    }

    private async Task<long> SeedAndGetTestSectionId(string name, string displayName, string color)
    {
        using var scope = factory.Services.CreateScope();
        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();

        return await sectionsService.CreateSection(name, displayName, color, CancellationToken.None);
    }

    private async Task<long> SeedTestSectionWithNotes()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();

        var sectionId = await sectionsService.CreateSection("SectionWithNotes", "Section With Notes", "#333333", CancellationToken.None);

        var section = await dbContext.Sections.FindAsync(sectionId);

        if (section != null)
        {
            var note = new NoteEntity
            {
                Title = "Test Note",
                Content = "Test Content",
                Section = section.Name
            };

            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        return sectionId;
    }

    private async Task<List<SectionsItemResponse>> GetAllSections()
    {
        var response = await client.GetAsync("/api/sections");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<SectionsItemResponse>>() ?? [];
    }
}
