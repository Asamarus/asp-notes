using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Tests.Controllers;

public class BooksControllerIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    private readonly ApiFactory factory;

    public BooksControllerIntegrationTests(ApiFactory factory) 
    {
        this.factory = factory;
        client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());
    }

    [Fact]
    public async Task Get_ShouldReturnOkResult_WithBooksList()
    {
        // Arrange
        await SeedTestBooksDataAsync();

        // Act
        var response = await client.GetAsync("/api/books");

        // Assert
        response.EnsureSuccessStatusCode();
        var booksResponse = await response.Content.ReadFromJsonAsync<List<ItemNameCountResponse>>();

        Assert.NotNull(booksResponse);
        Assert.NotEmpty(booksResponse);

        var books = booksResponse.ToList();
        Assert.Equal(2, books.Count);
        Assert.Contains(books, b => b.Name == "TestBook1" && b.Count == 1);
        Assert.Contains(books, b => b.Name == "TestBook2" && b.Count == 2);
    }

    [Fact]
    public async Task Get_ShouldReturnValidationProblem_WhenSectionIsInvalid()
    {
        // Arrange
        var invalidSection = "InvalidSection";

        // Act
        var response = await client.GetAsync($"/api/books?section={invalidSection}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal($"Section '{invalidSection}' is not valid!", problemDetails.Title);
    }

    private async Task SeedTestBooksDataAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();
        var booksService = scope.ServiceProvider.GetRequiredService<IBooksService>();

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

        await booksService.UpdateNoteBook(note1.Id, "TestBook1", CancellationToken.None);
        await booksService.UpdateNoteBook(note2.Id, "TestBook2", CancellationToken.None);
        await booksService.UpdateNoteBook(note3.Id, "TestBook2", CancellationToken.None);
    }
}
