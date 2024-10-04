using AspNotes.Core.Common;
using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Section.Models;
using AspNotes.Web.Models.Sections;
using AspNotes.Web.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace AspNotes.Web.Tests.Controllers;
public class HomeControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
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
    public async Task Index_ReturnsCorrectTitle()
    {
        // Arrange
        var configuration = TestHelper.GetConfiguration();
        var expectedTitle = configuration["ApplicationTitle"];

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();

        var testSections = new List<SectionEntity>
        {
            new() { Id = 1, Name = "Section1", DisplayName = "Display Name 1", Color = "Red", Position = 1 },
            new() { Id = 2, Name = "Section2", DisplayName = "Display Name 2", Color = "Blue", Position = 2 }
        };

        dbContext.Sections.AddRange(testSections);
        await dbContext.SaveChangesAsync();

        var expectedPreloadedState = new
        {
            Sections = new {
                AllNotesSection = TestHelper.GetAllNotesSection(),
                List = testSections.Select(x => new SectionItemResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Color = x.Color
                }).ToList()
            }
        };
        var expectedPreloadedStateJson = expectedPreloadedState.SerializeJson();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains($"<title>{expectedTitle}</title>", responseString);
        var preloadedStateStartIndex = responseString.IndexOf("window.__PRELOADED_STATE__ = ") + "window.__PRELOADED_STATE__ = ".Length;
        var preloadedStateEndIndex = responseString.IndexOf(';', preloadedStateStartIndex);
        var actualPreloadedStateJson = responseString[preloadedStateStartIndex..preloadedStateEndIndex].Trim();

        Assert.Equal(expectedPreloadedStateJson, actualPreloadedStateJson);
    }
}
