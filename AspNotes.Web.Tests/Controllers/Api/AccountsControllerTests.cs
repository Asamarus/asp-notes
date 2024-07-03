using AspNotes.Core.User;
using AspNotes.Web.Controllers.Api;
using AspNotes.Web.Models;
using AspNotes.Web.Models.Accounts;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace AspNotes.Web.Tests.Controllers.Api;

public class AccountsControllerTests : DatabaseTestBase
{
    private readonly Mock<IOptions<JwtSettings>> _mockJwtSettingsOption;
    private readonly Mock<IWebHostEnvironment> _mockEnv;
    private readonly JwtSettings _jwtSettings = TestHelper.GetJwtSettings();

    public AccountsControllerTests()
    {
        _mockJwtSettingsOption = new Mock<IOptions<JwtSettings>>();
        _mockJwtSettingsOption.Setup(m => m.Value).Returns(_jwtSettings);
        _mockEnv = new Mock<IWebHostEnvironment>();
        _mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Development);
    }

    [Fact]
    public async Task Login_ReturnsBadRequestResult_WithInvalidCredentials()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);
        var controller = new AccountsController(userService, _mockJwtSettingsOption.Object, _mockEnv.Object);

        // Act
        var result = await controller.Login(new LoginRequest { Email = "invalid@example.com", Password = "password" }, null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid email or password", response.Message);
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithValidCredentials()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);
        (var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();

        var controller = new AccountsController(userService, _mockJwtSettingsOption.Object, _mockEnv.Object);
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await controller.Login(new LoginRequest { Email = userEmail, Password = userPassword }, 1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal(userEmail, response.Email);
        Assert.False(string.IsNullOrEmpty(response.Id));
        Assert.False(string.IsNullOrEmpty(response.Token));

        Assert.True(httpContext.Response.Headers.ContainsKey("Set-Cookie"));
        var cookies = httpContext.Response.Headers.SetCookie.Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(_jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.Equal(response.Token, cookie.Value.ToString());
        Assert.True(cookie.Secure);
        Assert.True(cookie.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
        Assert.True(cookie.MaxAge.HasValue);
        Assert.True(cookie.MaxAge.Value.TotalMinutes == _jwtSettings.AccessTokenExpirationMinutes);
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithValidCredentialsAndNoToken()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);
        (var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();

        var controller = new AccountsController(userService, _mockJwtSettingsOption.Object, _mockEnv.Object);
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await controller.Login(new LoginRequest { Email = userEmail, Password = userPassword });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal(userEmail, response.Email);
        Assert.False(string.IsNullOrEmpty(response.Id));
        Assert.Null(response.Token);

        Assert.True(httpContext.Response.Headers.ContainsKey("Set-Cookie"));
        var cookies = httpContext.Response.Headers.SetCookie.Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(_jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.False(string.IsNullOrEmpty(cookie.Value.ToString()));
        Assert.True(cookie.Secure);
        Assert.True(cookie.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
        Assert.True(cookie.MaxAge.HasValue);
        Assert.True(cookie.MaxAge.Value.TotalMinutes == _jwtSettings.AccessTokenExpirationMinutes);
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithValidCredentialsAndNoTokenInProductionEnvironment()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);
        (var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();

        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Production);
        var controller = new AccountsController(userService, _mockJwtSettingsOption.Object, mockEnv.Object);
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await controller.Login(new LoginRequest { Email = userEmail, Password = userPassword }, 1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal(userEmail, response.Email);
        Assert.False(string.IsNullOrEmpty(response.Id));
        Assert.Null(response.Token);

        Assert.True(httpContext.Response.Headers.ContainsKey("Set-Cookie"));
        var cookies = httpContext.Response.Headers.SetCookie.Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(_jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.False(string.IsNullOrEmpty(cookie.Value.ToString()));
        Assert.True(cookie.Secure);
        Assert.True(cookie.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
        Assert.True(cookie.MaxAge.HasValue);
        Assert.True(cookie.MaxAge.Value.TotalMinutes == _jwtSettings.AccessTokenExpirationMinutes);
    }

    [Fact]
    public async Task GetUser_ReturnsOkResult_WithUserDetails()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);

        var controller = new AccountsController(userService, _mockJwtSettingsOption.Object, _mockEnv.Object);
        var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await controller.GetUser();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserResponse>(okResult.Value);
        var (email, password) = TestHelper.GetDefaultUserCredentials();
        Assert.Equal(email, response.Email);
        Assert.False(string.IsNullOrEmpty(response.Id));
    }

    [Fact]
    public void Logout_Deletes_Cookie()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);
        var cookieName = _jwtSettings.CookieName;
        var controller = new AccountsController(userService, _mockJwtSettingsOption.Object, _mockEnv.Object);
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Add a cookie to the response
        httpContext.Response.Cookies.Append(cookieName, "test value");

        // Act
        var result = controller.Logout();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SuccessResponse>(okResult.Value);
        Assert.Equal("You are logged out!", response.Message);

        Assert.True(httpContext.Response.Headers.ContainsKey("Set-Cookie"));
        var cookies = httpContext.Response.Headers.SetCookie.Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(_jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.Equal(_jwtSettings.CookieName, cookie.Name.Value);
        Assert.True(cookie.Expires.HasValue && cookie.Expires.Value < DateTime.Now);//cookie was deleted
    }

    [Fact]
    public async Task ChangePassword_ReturnsBadRequestResult_WithInvalidCurrentPassword()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);

        var controller = new AccountsController(userService, _mockJwtSettingsOption.Object, _mockEnv.Object);
        var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await controller.ChangePassword(new ChangePasswordRequest { CurrentPassword = "invalid", NewPassword = "new" });

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid current password", response.Message);
        Assert.Equal("CurrentPassword", response.Field);
    }

    [Fact]
    public async Task ChangePassword_ReturnsOkResult_WithValidCurrentPassword()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);
        (var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();

        var controller = new AccountsController(userService, _mockJwtSettingsOption.Object, _mockEnv.Object);
        var httpContext = new DefaultHttpContext { User = TestHelper.GetTestUserClaimsPrincipal() };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await controller.ChangePassword(new ChangePasswordRequest { CurrentPassword = userPassword, NewPassword = "new" });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SuccessResponse>(okResult.Value);
        Assert.Equal("Password is changed!", response.Message);

        var user = await userService.GetUserByEmail(userEmail);
        Assert.NotNull(user);
        var hashedNewPassword = UsersHelper.HashPassword("new", user.Salt);
        Assert.Equal(hashedNewPassword, user.PasswordHash);
    }
}
