using AspNotes.Web.Controllers.Api;
using AspNotes.Web.Models;
using AspNotes.Web.Models.Application;
using AspNotes.Web.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace AspNotes.Web.Tests.Controllers.Api;

public class ApplicationControllerTests
{
    private readonly Mock<IOptions<AllNotesSection>> _mockAllNotesSectionOption;
    private readonly AllNotesSection _allNotesSection = TestHelper.GetAllNotesSection();

    public ApplicationControllerTests()
    {
        _mockAllNotesSectionOption = new Mock<IOptions<AllNotesSection>>();
        _mockAllNotesSectionOption.Setup(m => m.Value).Returns(_allNotesSection);
    }

    [Fact]
    public void GetInitialData_ReturnsInitialDataResponse()
    {
        // Arrange
        var controller = new ApplicationController(_mockAllNotesSectionOption.Object);
        var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = controller.GetInitialData();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<InitialDataResponse>(okResult.Value);
        returnValue.AllNotesSection.Should().BeEquivalentTo(_allNotesSection);
    }
}
