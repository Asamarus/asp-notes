using AspNotes.Core.Section;
using AspNotes.Core.Section.Models;
using AspNotes.Web.Controllers;
using AspNotes.Web.Models;
using AspNotes.Web.Models.Sections;
using AspNotes.Web.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace AspNotes.Web.Tests.Controllers;
public class HomeControllerTests
{
    private readonly Mock<IOptions<AllNotesSection>> _mockAllNotesSectionOption;
    private readonly AllNotesSection _allNotesSection = TestHelper.GetAllNotesSection();

    public HomeControllerTests()
    {
        _mockAllNotesSectionOption = new Mock<IOptions<AllNotesSection>>();
        _mockAllNotesSectionOption.Setup(m => m.Value).Returns(_allNotesSection);
    }

    [Fact]
    public async Task Index_ReturnsCorrectTitleAndPreloadedState()
    {
        // Arrange
        var configuration = TestHelper.GetConfiguration();
        var mockService = new Mock<ISectionsService>();
        var expectedSections = new List<SectionDto>
        {
            new() { Id = 1, Name = "Section1", DisplayName = "Display Name 1", Color = "Red", Position = 1 },
            new() { Id = 2, Name = "Section2", DisplayName = "Display Name 2", Color = "Blue", Position = 2 }
        };
        mockService.Setup(service => service.GetSections()).ReturnsAsync(expectedSections);
        var controller = new HomeController(configuration, _mockAllNotesSectionOption.Object, mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var expectedTitle = configuration["ApplicationTitle"];
        Assert.Equal(expectedTitle, viewResult.ViewData["Title"]);

        var preloadedState = viewResult.ViewData["PreloadedState"];
        Assert.NotNull(preloadedState);

        var expectedPreloadedState = new
        {
            Sections = new {
                AllNotesSection = _allNotesSection,
                List = expectedSections.Select(x => new SectionItemResponse(x)).ToList()
            }
        };

        preloadedState.Should().BeEquivalentTo(expectedPreloadedState);
    }
}
