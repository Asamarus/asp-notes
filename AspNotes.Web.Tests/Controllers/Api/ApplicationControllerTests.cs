using AspNotes.Web.Controllers.Api;
using AspNotes.Web.Models.Application;
using AspNotes.Web.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Tests.Controllers.Api;

public class ApplicationControllerTests
{
    [Fact]
    public void GetInitialData_ReturnsInitialDataResponse()
    {
        // Arrange
        var controller = new ApplicationController();
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
        Assert.Equal("Project template title", returnValue.Title);
        Assert.Equal("Some data", returnValue.SomeData);
    }
}
