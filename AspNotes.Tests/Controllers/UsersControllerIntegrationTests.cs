using AspNotes.Entities;
using AspNotes.Helpers;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Tests.Controllers;

public class UsersControllerIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    private readonly ApiFactory factory;
    private readonly JwtSettings jwtSettings;

    public UsersControllerIntegrationTests(ApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var jwtSettingsOptions = scope.ServiceProvider.GetRequiredService<IOptions<JwtSettings>>();
        jwtSettings = jwtSettingsOptions.Value;
    }

    [Fact]
    public async Task Login_ShouldReturnCreated_WhenCredentialsAreValid()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var loginRequest = new UsersLoginRequest
        {
            Email = user.Email,
            Password = "TestPassword123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<UsersLoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.Equal(user.Id, loginResponse.Id);
        Assert.Equal(user.Email, loginResponse.Email);
        Assert.Null(loginResponse.AccessToken);
    }

    [Fact]
    public async Task Login_ShouldReturnTokenInDevelopment_WhenRequested()
    {
        // Arrange
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(Environments.Development);
        });

        var devClient = factory.CreateClient();

        var user = await CreateTestUserAsync();
        var loginRequest = new UsersLoginRequest
        {
            Email = user.Email,
            Password = "TestPassword123"
        };

        // Act
        var response = await devClient.PostAsJsonAsync("/api/users/login?withToken=1", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<UsersLoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.Equal(user.Id, loginResponse.Id);
        Assert.Equal(user.Email, loginResponse.Email);
        Assert.NotNull(loginResponse.AccessToken);

        Assert.True(response.Headers.Contains("Set-Cookie"));
        var cookies = response.Headers.GetValues("Set-Cookie").Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.Equal(loginResponse.AccessToken, cookie.Value.ToString());
        Assert.True(cookie.Secure);
        Assert.True(cookie.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
        Assert.True(cookie.MaxAge.HasValue);
        Assert.True(cookie.MaxAge.Value.TotalMinutes >= jwtSettings.AccessTokenExpirationMinutes);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WithInvalidCredentials()
    {
        // Arrange
        var loginRequest = new UsersLoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "InvalidPassword"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnOk_WhenAuthenticated()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var token = TestHelper.GenerateTestToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var userResponse = await response.Content.ReadFromJsonAsync<UsersCurrentUserResponse>();

        Assert.NotNull(userResponse);
        Assert.Equal(user.Id.ToString(), userResponse.Id);
        Assert.Equal(user.Email, userResponse.Email);
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnNoContent_WhenValidData()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var token = TestHelper.GenerateTestToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UsersChangePasswordRequest
        {
            CurrentPassword = "TestPassword123",
            NewPassword = "NewPassword456",
            PasswordRepeat = "NewPassword456"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/users/password", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify new password works
        client.DefaultRequestHeaders.Clear();

        var loginRequest = new UsersLoginRequest
        {
            Email = user.Email,
            Password = "NewPassword456"
        };

        var loginResponse = await client.PostAsJsonAsync("/api/users/login", loginRequest);
        Assert.Equal(HttpStatusCode.Created, loginResponse.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnBadRequest_WithInvalidCurrentPassword()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var token = TestHelper.GenerateTestToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UsersChangePasswordRequest
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPassword456",
            PasswordRepeat = "NewPassword456"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/users/password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new UsersChangePasswordRequest
        {
            CurrentPassword = "TestPassword123",
            NewPassword = "NewPassword456",
            PasswordRepeat = "NewPassword456"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/users/password", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ShouldDeleteAuthCookie()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var token = TestHelper.GenerateTestToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsync("/api/users/logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var cookies = response.Headers.GetValues("Set-Cookie").ToList();
        Assert.Contains(cookies, c => c.StartsWith($"{jwtSettings.CookieName}=") && c.Contains("expires="));
    }

    private async Task<UserEntity> CreateTestUserAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var salt = UsersHelper.GenerateSalt();
        var hashedPassword = UsersHelper.HashPassword("TestPassword123", salt);

        var email = $"test-{Guid.NewGuid()}@example.com";
        var user = new UserEntity
        {
            Email = email,
            Salt = salt,
            PasswordHash = hashedPassword,
            LastLogin = DateTime.UtcNow.AddDays(-1)
        };

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return user;
    }
}
