using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Tests.Controllers;

public class TagsControllerIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    private readonly ApiFactory factory;

    public TagsControllerIntegrationTests(ApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());
    }

    [Fact]
    public async Task Get_ShouldReturnOkResult_WithTagsList()
    {
        // Arrange
        await SeedTestTagsDataAsync();

        // Act
        var response = await client.GetAsync("/api/tags");

        // Assert
        response.EnsureSuccessStatusCode();
        var tagsResponse = await response.Content.ReadFromJsonAsync<List<ItemNameCountResponse>>();

        Assert.NotNull(tagsResponse);
        Assert.NotEmpty(tagsResponse);

        var tags = tagsResponse.ToList();
        Assert.Equal(2, tags.Count);
        Assert.Contains(tags, t => t.Name == "TestTag1" && t.Count == 1);
        Assert.Contains(tags, t => t.Name == "TestTag2" && t.Count == 2);
    }

    [Fact]
    public async Task Get_ShouldReturnValidationProblem_WhenSectionIsInvalid()
    {
        // Arrange
        var invalidSection = "InvalidSection";

        // Act
        var response = await client.GetAsync($"/api/tags?section={invalidSection}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal($"Section '{invalidSection}' is not valid!", problemDetails.Title);
    }

    private async Task SeedTestTagsDataAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();
        var tagsService = scope.ServiceProvider.GetRequiredService<ITagsService>();

        await sectionsService.CreateSection("TestSection", "Test Section", "#000000", CancellationToken.None);

        var note1 = new NoteEntity
        {
            Title = "TestNote1",
            Content = "TestNote1Content",
            Section = "TestSection",
        };

        var note2 = new NoteEntity
        {
            Title = "TestNote2",
            Content = "TestNote2Content",
            Section = "TestSection",
        };

        var note3 = new NoteEntity
        {
            Title = "TestNote3",
            Content = "TestNote3Content",
            Section = "TestSection",
        };

        await dbContext.Notes.AddRangeAsync(note1, note2, note3);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        await tagsService.UpdateNoteTags(note1.Id, ["TestTag1"], CancellationToken.None);
        await tagsService.UpdateNoteTags(note2.Id, ["TestTag2"], CancellationToken.None);
        await tagsService.UpdateNoteTags(note3.Id, ["TestTag2"], CancellationToken.None);
    }
}
