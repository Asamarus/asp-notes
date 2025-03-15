using AspNotes.Controllers;
using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace AspNotes.Tests.Controllers;

public class ApplicationDataControllerTests
{
    private readonly Mock<ISectionsService> sectionsServiceMock;
    private readonly Mock<IOptions<AllNotesSection>> allNotesSectionMock;
    private readonly ApplicationDataController controller;

    public ApplicationDataControllerTests()
    {
        sectionsServiceMock = new Mock<ISectionsService>();
        allNotesSectionMock = new Mock<IOptions<AllNotesSection>>();
        controller = new ApplicationDataController(sectionsServiceMock.Object, allNotesSectionMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnOkResult_WithApplicationDataResponse()
    {
        // Arrange
        var sections = new List<SectionEntity>
        {
            new() {
                Id = 1,
                Name = "Section1",
                DisplayName = "Section 1",
                Color = "Red",
                Position = 1
            },
            new() {
                Id = 2,
                Name = "Section2",
                DisplayName = "Section 2",
                Color = "Blue",
                Position = 2
            }
        };
        var allNotesSection = new AllNotesSection { 
            Name = "All Notes", 
            DisplayName = "All Notes", 
            Color = "Yellow" 
        };

        sectionsServiceMock.Setup(s => s.GetSections(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sections);
        allNotesSectionMock.Setup(a => a.Value)
            .Returns(allNotesSection);

        // Act
        var result = await controller.Get(CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var response = result.Value as ApplicationDataResponse;
        Assert.NotNull(response);
        Assert.Equal(allNotesSection, response.AllNotesSection);
        Assert.Equal(sections.Count, response.Sections.Count());
    }
}
