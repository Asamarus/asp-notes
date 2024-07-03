using AspNotes.Core.Common.Helpers;
using AspNotes.Web.Models.Accounts;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AspNotes.Web.Tests.Controllers.Api;

public class AccountsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    [Fact]
    public async Task Login_ReturnsProblem_WhenEmptyRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/accounts/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonDocument.Parse(responseString);
        bool hasTitle = responseObject.RootElement.TryGetProperty("title", out JsonElement titleElement);
        bool hasErrors = responseObject.RootElement.TryGetProperty("errors", out JsonElement errorsElement);

        Assert.True(hasTitle);
        Assert.True(hasErrors);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenInvalidCredentials()
    {
        // Arrange
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest { Email = "some@user.com", Password = "some password" };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/accounts/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(response.Content.Headers.ContentType?.ToString().Contains("application/json") ?? false);
        Assert.NotNull(response.Content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var errorResponse = responseContent.DeserializeJson<ErrorResponse>();

        Assert.NotNull(errorResponse);
        Assert.Equal("Invalid email or password", errorResponse.Message);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenValidCredentials()
    {
        // Arrange
        var jwtSettings = TestHelper.GetJwtSettings();
        (var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();
        using var factory = new CustomWebApplicationFactory<Startup>();
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest { Email = userEmail, Password = userPassword };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/accounts/login?withToken=1", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = responseContent.DeserializeJson<LoginResponse>();

        Assert.NotNull(loginResponse);
        Assert.Equal(userEmail, loginResponse.Email);
        Assert.False(string.IsNullOrEmpty(loginResponse.Id));
        Assert.False(string.IsNullOrEmpty(loginResponse.Token));

        Assert.True(response.Headers.Contains("Set-Cookie"));
        var cookies = response.Headers.GetValues("Set-Cookie").Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.Equal(loginResponse.Token, cookie.Value.ToString());
        Assert.True(cookie.Secure);
        Assert.True(cookie.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
        Assert.True(cookie.MaxAge.HasValue);
        Assert.True(cookie.MaxAge.Value.TotalMinutes == jwtSettings.AccessTokenExpirationMinutes);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenValidCredentialsAndNoToken()
    {
        // Arrange        
        var jwtSettings = TestHelper.GetJwtSettings();
        (var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest { Email = userEmail, Password = userPassword };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/accounts/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = responseContent.DeserializeJson<LoginResponse>();

        Assert.NotNull(loginResponse);
        Assert.Equal(userEmail, loginResponse.Email);
        Assert.False(string.IsNullOrEmpty(loginResponse.Id));
        Assert.True(string.IsNullOrEmpty(loginResponse.Token));

        Assert.True(response.Headers.Contains("Set-Cookie"));
        var cookies = response.Headers.GetValues("Set-Cookie").Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.False(string.IsNullOrEmpty(cookie.Value.ToString()));
        Assert.True(cookie.Secure);
        Assert.True(cookie.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
        Assert.True(cookie.MaxAge.HasValue);
        Assert.True(cookie.MaxAge.Value.TotalMinutes == jwtSettings.AccessTokenExpirationMinutes);
    }

    [Fact]
    public async Task Login_ReturnsNullToken_WhenValidCredentialsAndProductionEnvironment()
    {
        // Arrange
        var jwtSettings = TestHelper.GetJwtSettings();
        (var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();
        using var factory = new ProductionWebApplicationFactory<Startup>();
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest { Email = userEmail, Password = userPassword };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/accounts/login?withToken=1", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = responseContent.DeserializeJson<LoginResponse>();

        Assert.NotNull(loginResponse);
        Assert.Equal(userEmail, loginResponse.Email);
        Assert.False(string.IsNullOrEmpty(loginResponse.Id));
        Assert.Null(loginResponse.Token);

        Assert.True(response.Headers.Contains("Set-Cookie"));
        var cookies = response.Headers.GetValues("Set-Cookie").Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.False(string.IsNullOrEmpty(cookie.Value.ToString()));
        Assert.True(cookie.Secure);
        Assert.True(cookie.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
        Assert.True(cookie.MaxAge.HasValue);
        Assert.True(cookie.MaxAge.Value.TotalMinutes == jwtSettings.AccessTokenExpirationMinutes);
    }

    [Fact]
    public async Task GetUser_ReturnsOk_WhenUserExist()
    {
        // Arrange
        var client = factory.CreateClient();
        var (email, password) = TestHelper.GetDefaultUserCredentials();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        // Act
        var response = await client.PostAsync("/api/accounts/getUser", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);

        var responseString = await response.Content.ReadAsStringAsync();
        var userResponse = responseString.DeserializeJson<UserResponse>();

        Assert.NotNull(userResponse);
        Assert.Equal(email, userResponse.Email);
        Assert.False(string.IsNullOrEmpty(userResponse.Id));
    }

    [Fact]
    public async Task GetUser_ReturnsUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsync("/api/accounts/getUser", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_Deletes_Cookie()
    {
        // Arrange
        var token = TestHelper.GetTestUserToken();
        var jwtSettings = TestHelper.GetJwtSettings();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add a cookie to the request
        client.DefaultRequestHeaders.Add("Cookie", $"{jwtSettings.CookieName}=test value");

        // Act
        var response = await client.PostAsync("/api/accounts/logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var logoutResponse = responseString.DeserializeJson<SuccessResponse>();

        Assert.NotNull(logoutResponse);
        Assert.Equal("You are logged out!", logoutResponse.Message);

        Assert.True(response.Headers.Contains("Set-Cookie"));
        var cookies = response.Headers.GetValues("Set-Cookie").Select(x => Microsoft.Net.Http.Headers.SetCookieHeaderValue.Parse(x)).ToList();
        var cookie = cookies.Find(x => x.Name.Value != null && x.Name.Value.Equals(jwtSettings.CookieName));

        Assert.NotNull(cookie);
        Assert.Equal(jwtSettings.CookieName, cookie.Name.Value);
        Assert.True(cookie.Expires.HasValue && cookie.Expires.Value < DateTime.Now);//cookie was deleted
    }

    [Fact]
    public async Task ChangePassword_ReturnsBadRequest_WhenInvalidRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var changePasswordRequest = new ChangePasswordRequest { CurrentPassword = "short", NewPassword = "123456", PasswordRepeat = "1234567" };
        var content = new StringContent(JsonSerializer.Serialize(changePasswordRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/accounts/changePassword", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Content);

        var responseString = await response.Content.ReadAsStringAsync();
        var problemDetails = responseString.DeserializeJson<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("One or more validation errors occurred.", problemDetails.Title);
        Assert.Equal(400, problemDetails.Status);
        Assert.NotNull(problemDetails.Errors);
        Assert.Equal(2, problemDetails.Errors.Count);
        Assert.True(problemDetails.Errors.ContainsKey("CurrentPassword"));
        Assert.Equal("Minimum password length is 6", problemDetails.Errors["CurrentPassword"][0]);
        Assert.True(problemDetails.Errors.ContainsKey("PasswordRepeat"));
        Assert.Equal("The new password and confirmation password do not match", problemDetails.Errors["PasswordRepeat"][0]);
    }

    [Fact]
    public async Task ChangePassword_ReturnsBadRequest_WhenInvalidCurrentPassword()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var changePasswordRequest = new ChangePasswordRequest { CurrentPassword = "invalid", NewPassword = "123456", PasswordRepeat = "123456" };
        var content = new StringContent(JsonSerializer.Serialize(changePasswordRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/accounts/changePassword", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(response.Content.Headers.ContentType?.ToString().Contains("application/json") ?? false);
        Assert.NotNull(response.Content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var errorResponse = responseContent.DeserializeJson<ErrorResponse>();

        Assert.NotNull(errorResponse);
        Assert.Equal("Invalid current password", errorResponse.Message);
    }

    [Fact]
    public async Task ChangePassword_ReturnsOk_WhenValidRequest()
    {
        // Arrange
        (var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());

        var changePasswordRequest = new ChangePasswordRequest { CurrentPassword = userPassword, NewPassword = "new_password_123", PasswordRepeat = "new_password_123" };
        var content = new StringContent(JsonSerializer.Serialize(changePasswordRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/accounts/changePassword", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var successResponse = responseContent.DeserializeJson<SuccessResponse>();

        Assert.NotNull(successResponse);
        Assert.Equal("Password is changed!", successResponse.Message);
    }
}
