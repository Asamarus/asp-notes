using AspNotes.Web.Controllers.Api;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Sources;
using AspNotes.Web.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Tests.Controllers.Api;

public class SourcesControllerTests
{
    //[Fact]
    //public async Task AddNoteSource_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new SourcesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.AddNoteSource(AddNoteSourceRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<AddNoteSourceResponse>(okResult.Value);
    //    var response = AddNoteSourceResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task UpdateNoteSource_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new SourcesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.UpdateNoteSource(UpdateNoteSourceRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<UpdateNoteSourceResponse>(okResult.Value);
    //    var response = UpdateNoteSourceResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task RemoveNoteSource_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new SourcesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.RemoveNoteSource(RemoveNoteSourceRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<RemoveNoteSourceResponse>(okResult.Value);
    //    var response = RemoveNoteSourceResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task ReorderNoteSources_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new SourcesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.ReorderNoteSources(ReorderNoteSourcesRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<ReorderNoteSourcesResponse>(okResult.Value);
    //    var response = ReorderNoteSourcesResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}
}