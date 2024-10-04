using AspNotes.Web.Controllers.Api;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Tags;
using AspNotes.Web.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Tests.Controllers.Api;

public class TagsControllerTests
{
    //[Fact]
    //public async Task GetTagsList_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new TagsController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.GetTagsList(GetTagsListRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<GetTagsListResponse>(okResult.Value);
    //    var response = GetTagsListResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task AutocompleteTags_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new TagsController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.AutocompleteTags(AutocompleteTagsRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<AutocompleteTagsResponse>(okResult.Value);
    //    var response = AutocompleteTagsResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}
}