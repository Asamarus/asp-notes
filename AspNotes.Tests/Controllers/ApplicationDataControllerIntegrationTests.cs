using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Tests.Controllers;

public class ApplicationDataControllerIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    private readonly ApiFactory factory;

    public ApplicationDataControllerIntegrationTests(ApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());
    }

    [Fact]
    public async Task Get_ShouldReturnOkResult_WithApplicationDataResponse()
    {
        // Arrange
        await SeedTestSectionsDataAsync();

        // Act
        var response = await client.GetAsync("/api/application-data");

        // Assert
        response.EnsureSuccessStatusCode();
        var applicationDataResponse = await response.Content.ReadFromJsonAsync<ApplicationDataResponse>();

        Assert.NotNull(applicationDataResponse);
        Assert.NotNull(applicationDataResponse.AllNotesSection);
        Assert.NotEmpty(applicationDataResponse.Sections);

        var sections = applicationDataResponse.Sections.ToList();
        Assert.Equal(3, sections.Count);
        Assert.Contains(sections, s => s.Name == "IntegrationTestSection1");
        Assert.Contains(sections, s => s.Name == "IntegrationTestSection2");
        Assert.Contains(sections, s => s.Name == "IntegrationTestSection3");
    }

    private async Task SeedTestSectionsDataAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        // Clear existing sections first to ensure a clean state
        dbContext.Sections.RemoveRange(dbContext.Sections.ToList());
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Add test sections specific to this test
        dbContext.Sections.AddRange(
            new SectionEntity
            {
                Name = "IntegrationTestSection1",
                DisplayName = "Integration Test Section 1",
                Color = "#FF5733",
                Position = 1
            },
            new SectionEntity
            {
                Name = "IntegrationTestSection2",
                DisplayName = "Integration Test Section 2",
                Color = "#33FF57",
                Position = 2
            },
            new SectionEntity
            {
                Name = "IntegrationTestSection3",
                DisplayName = "Integration Test Section 3",
                Color = "#3357FF",
                Position = 3
            }
        );

        await dbContext.SaveChangesAsync(CancellationToken.None);
    }
}
