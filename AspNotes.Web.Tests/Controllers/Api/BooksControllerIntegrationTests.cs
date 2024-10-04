using AspNotes.Core.Book.Models;
using AspNotes.Core.Common;
using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Note.Models;
using AspNotes.Core.Section.Models;
using AspNotes.Web.Models.Books;
using AspNotes.Web.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Web.Tests.Controllers.Api;

public class BooksControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
{
    public void Dispose()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
        
        TestHelper.ClearDatabase(dbContext);
        GC.SuppressFinalize(this);
    }

    //[Fact]
    //public async Task GetBooksList_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    using var scope = factory.Services.CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();

    //    dbContext.Sections.Add(new SectionEntity {
    //        Name = "Section1",
    //        DisplayName = "Section1",
    //        Color = "#0000",
    //        Position = 1
    //    });

    //    dbContext.Notes.Add(new NoteEntity
    //    {
    //        Section = "Section1",
    //        Book = "Book1",
    //    });

    //    dbContext.Notes.Add(new NoteEntity
    //    {
    //        Section = "Section1",
    //        Book = "Book1",
    //    });

    //    dbContext.Notes.Add(new NoteEntity
    //    {
    //        Section = "Section1",
    //        Book = "Book2",
    //    });

    //    var testBooks = new List<BookEntity>
    //    {
    //        new() { Name = "Book1", Section = "Section1" },
    //        new() { Name = "Book2", Section = "Section1" }
    //    };

    //    dbContext.Books.AddRange(testBooks);
    //    await dbContext.SaveChangesAsync();

    //    var getBooksListRequest = new GetBooksListRequest
    //    {
    //        Section = "Section1"
    //    };

    //    // Act
    //    var response = await client.PostAsync("/api/books/getList", JsonContent.Create(getBooksListRequest));

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<List<BookItemResponse>>();

    //    Assert.NotNull(responseData);
    //    Assert.Equal(2, responseData.Count);
    //    var book = responseData[0];
    //    Assert.Equal("Book1", book.Name);
    //    Assert.Equal(2, book.Count);

    //    book = responseData[1];
    //    Assert.Equal("Book2", book.Name);
    //    Assert.Equal(1, book.Count);
    //}

    //[Fact]
    //public async Task AutocompleteBooks_ReturnsOk()
    //{
    //    // Arrange
    //    var client = factory.CreateClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

    //    using var scope = factory.Services.CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();

    //    dbContext.Sections.Add(new SectionEntity
    //    {
    //        Name = "Section1",
    //        DisplayName = "Section1",
    //        Color = "#0000",
    //        Position = 1
    //    });

    //    var testBooks = new List<BookEntity>
    //    {
    //        new() { Name = "Book1", Section = "Section1" },
    //        new() { Name = "Book2", Section = "Section1" }
    //    };

    //    dbContext.Books.AddRange(testBooks);
    //    await dbContext.SaveChangesAsync();

    //    var autocompleteRequest = new AutocompleteBooksRequest
    //    {
    //        SearchTerm = "Book",
    //        Section = "Section1"
    //    };

    //    // Act
    //    var response = await client.PostAsync("/api/books/autocomplete", JsonContent.Create(autocompleteRequest));

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    Assert.NotNull(response.Content);

    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    var responseData = responseContent.DeserializeJson<List<string>>();

    //    Assert.NotNull(responseData);
    //    Assert.Equal(2, responseData.Count);
    //    Assert.Equal("Book1", responseData[0]);
    //    Assert.Equal("Book2", responseData[1]);
    //}
}
