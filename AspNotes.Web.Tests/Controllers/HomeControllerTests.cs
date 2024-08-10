using AspNotes.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Tests.Controllers;
public class HomeControllerTests
{
    [Fact]
    public void Index_ReturnsCorrectTitle()
    {
        // Arrange
        var controller = new HomeController();
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Notes", viewResult.ViewData["Title"]);
    }
}
