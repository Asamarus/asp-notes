using AspNotes.Controllers;
using AspNotes.Entities;
using AspNotes.Helpers;
using AspNotes.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace AspNotes.Tests.Controllers;

public class UsersControllerTests : InMemoryDatabaseTestBase
{
    private readonly Mock<IOptions<JwtSettings>> jwtSettingsOptionsMock;
    private readonly Mock<IWebHostEnvironment> webHostEnvironmentMock;
    private readonly UsersController controller;
    private readonly JwtSettings jwtSettings;
    private readonly HttpContext httpContext = new DefaultHttpContext();

    public UsersControllerTests()
    {
        jwtSettingsOptionsMock = new Mock<IOptions<JwtSettings>>();
        webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

        jwtSettings = new JwtSettings
        {
            Secret = "SampleSecretKeyForTestingPurposes123456789012345678901234",
            ValidIssuer = "test-issuer",
            ValidAudience = "test-audience",
            AccessTokenExpirationMinutes = 60,
            CookieName = "test-auth-cookie"
        };

        jwtSettingsOptionsMock.Setup(x => x.Value).Returns(jwtSettings);

        controller = new UsersController(
            DbContext,
            jwtSettingsOptionsMock.Object,
            webHostEnvironmentMock.Object
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Login_ShouldReturnCreatedResult_WhenCredentialsAreValid()
    {
        // Arrange
        var salt = UsersHelper.GenerateSalt();
        var passwordHash = UsersHelper.HashPassword("password123", salt);
        var userEntity = new UserEntity
        {
            Id = 2,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Salt = salt,
            LastLogin = DateTime.UtcNow.AddDays(-1)
        };

        DbContext.Users.Add(userEntity);
        await DbContext.SaveChangesAsync();

        var request = new UsersLoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var result = await controller.Login(request, null, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);

        var response = Assert.IsType<UsersLoginResponse>(createdResult.Value);
        Assert.Equal(userEntity.Id, response.Id);
        Assert.Equal(userEntity.Email, response.Email);
        Assert.Null(response.AccessToken);
    }

    [Fact]
    public async Task Login_ShouldReturnAccessToken_WhenWithTokenAndDevelopmentEnvironment()
    {
        // Arrange
        var salt = UsersHelper.GenerateSalt();
        var passwordHash = UsersHelper.HashPassword("password123", salt);
        var userEntity = new UserEntity
        {
            Id = 2,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Salt = salt,
            LastLogin = DateTime.UtcNow.AddDays(-1)
        };

        DbContext.Users.Add(userEntity);
        await DbContext.SaveChangesAsync();

        var request = new UsersLoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        webHostEnvironmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        // Act
        var result = await controller.Login(request, 1, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);

        var response = Assert.IsType<UsersLoginResponse>(createdResult.Value);
        Assert.Equal(userEntity.Id, response.Id);
        Assert.Equal(userEntity.Email, response.Email);
        Assert.NotNull(response.AccessToken);

        Assert.True(httpContext.Response.Headers.ContainsKey("Set-Cookie"));
        var cookies = httpContext.Response.Headers.SetCookie.Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.Equal(response.AccessToken, cookie.Value.ToString());
        Assert.True(cookie.Secure);
        Assert.True(cookie.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
        Assert.True(cookie.MaxAge.HasValue);
        Assert.True(cookie.MaxAge.Value.TotalMinutes >= jwtSettings.AccessTokenExpirationMinutes);
    }

    [Fact]
    public async Task Login_ShouldReturnValidationProblem_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new UsersLoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        // Act
        var result = await controller.Login(request, null, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnValidationProblem_WhenPasswordIsInvalid()
    {
        // Arrange
        var salt = UsersHelper.GenerateSalt();
        var passwordHash = UsersHelper.HashPassword("correctpassword", salt);
        var userEntity = new UserEntity
        {
            Id = 2,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Salt = salt
        };

        var request = new UsersLoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        DbContext.Users.Add(userEntity);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await controller.Login(request, null, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public void Logout_ShouldDeleteAuthCookie_AndReturnNoContent()
    {
        // Act
        var result = controller.Logout();

        // Assert
        Assert.IsType<NoContentResult>(result);

        Assert.True(httpContext.Response.Headers.ContainsKey("Set-Cookie"));
        var cookies = httpContext.Response.Headers.SetCookie.Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.Equal(jwtSettings.CookieName, cookie.Name.Value);
        Assert.True(cookie.Expires.HasValue && cookie.Expires.Value < DateTime.Now);//cookie was deleted
    }

    [Fact]
    public void GetCurrentUser_ShouldReturnOkWithUserInfo_WhenClaimsAreValid()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "test@example.com"),
            new(ClaimTypes.NameIdentifier, "1")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = controller.GetCurrentUser();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UsersCurrentUserResponse>(okResult.Value);
        Assert.Equal("1", response.Id);
        Assert.Equal("test@example.com", response.Email);
    }

    [Fact]
    public void GetCurrentUser_ShouldReturnUnauthorized_WhenClaimsAreMissing()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "test@example.com")
            // Missing NameIdentifier claim
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = controller.GetCurrentUser();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnNoContent_WhenDataIsValid()
    {
        // Arrange
        var salt = UsersHelper.GenerateSalt();
        var currentPasswordHash = UsersHelper.HashPassword("currentPassword", salt);
        var userId = "2";
        var userEntity = new UserEntity
        {
            Id = long.Parse(userId),
            Email = "test@example.com",
            PasswordHash = currentPasswordHash,
            Salt = salt
        };

        DbContext.Users.Add(userEntity);
        await DbContext.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var request = new UsersChangePasswordRequest
        {
            CurrentPassword = "currentPassword",
            PasswordRepeat = "newPassword",
            NewPassword = "newPassword"
        };

        // Act
        var result = await controller.ChangePassword(request, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify the password was changed
        userEntity = await DbContext.Users.FindAsync(long.Parse(userId));
        Assert.NotNull(userEntity);
        Assert.Equal(UsersHelper.HashPassword("newPassword", salt), userEntity.PasswordHash);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        // Arrange
        var request = new UsersChangePasswordRequest
        {
            CurrentPassword = "currentPassword",
            PasswordRepeat = "newPassword",
            NewPassword = "newPassword"
        };

        // Act
        var result = await controller.ChangePassword(request, CancellationToken.None);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "999";
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var request = new UsersChangePasswordRequest
        {
            CurrentPassword = "currentPassword",
            PasswordRepeat = "newPassword",
            NewPassword = "newPassword"
        };

        // Act
        var result = await controller.ChangePassword(request, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnValidationProblem_WhenInvalidCurrentPassword()
    {
        // Arrange
        var salt = UsersHelper.GenerateSalt();
        var currentPasswordHash = UsersHelper.HashPassword("correctPassword", salt);
        var userId = "2";
        var userEntity = new UserEntity
        {
            Id = long.Parse(userId),
            Email = "test@example.com",
            PasswordHash = currentPasswordHash,
            Salt = salt
        };

        DbContext.Users.Add(userEntity);
        await DbContext.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var request = new UsersChangePasswordRequest
        {
            CurrentPassword = "wrongPassword",
            PasswordRepeat = "newPassword",
            NewPassword = "newPassword"
        };

        // Act
        var result = await controller.ChangePassword(request, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
}
