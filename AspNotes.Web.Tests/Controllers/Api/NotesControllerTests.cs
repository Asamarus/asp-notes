using AspNotes.Web.Controllers.Api;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Notes;
using AspNotes.Web.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Tests.Controllers.Api;

public class NotesControllerTests
{
    //[Fact]
    //public async Task SearchNotes_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.SearchNotes(SearchNotesRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<SearchNotesResponse>(okResult.Value);
    //    var response = SearchNotesResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task AutocompleteNotes_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.AutocompleteNotes(AutocompleteNotesRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<AutocompleteNotesResponse>(okResult.Value);
    //    var response = AutocompleteNotesResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task GetNote_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.GetNote(GetNoteRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<GetNoteResponse>(okResult.Value);
    //    var response = GetNoteResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task CreateNote_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.CreateNote(CreateNoteRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<CreateNoteResponse>(okResult.Value);
    //    var response = CreateNoteResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task UpdateNote_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.UpdateNote(UpdateNoteRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<UpdateNoteResponse>(okResult.Value);
    //    var response = UpdateNoteResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task UpdateNoteBook_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.UpdateNoteBook(UpdateNoteBookRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<UpdateNoteBookResponse>(okResult.Value);
    //    var response = UpdateNoteBookResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task UpdateNoteTags_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.UpdateNoteTags(UpdateNoteTagsRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<UpdateNoteTagsResponse>(okResult.Value);
    //    var response = UpdateNoteTagsResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task UpdateNoteSection_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.UpdateNoteSection(UpdateNoteSectionRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<UpdateNoteSectionResponse>(okResult.Value);
    //    var response = UpdateNoteSectionResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task DeleteNote_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.DeleteNote(DeleteNoteRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<DeleteNoteResponse>(okResult.Value);
    //    var response = DeleteNoteResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}

    //[Fact]
    //public async Task GetCalendarDays_ReturnsOkResponse()
    //{
    //    // Arrange
    //    var controller = new NotesController();
    //    var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
    //    controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

    //    // Act
    //    var result = await controller.GetCalendarDays(GetNoteCalendarDaysRequestMock.Get());

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var returnValue = Assert.IsType<NoteCalendarDaysResponseItem>(okResult.Value);
    //    var response = GetNoteCalendarDaysResponseMock.Get();
    //    Assert.Equal(response.Data, returnValue.Data);
    //}
}
