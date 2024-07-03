using Ardalis.GuardClauses;
using AspNotes.Core.User;
using AspNotes.Web.Helpers;
using AspNotes.Web.Models;
using AspNotes.Web.Models.Accounts;
using AspNotes.Web.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNotes.Web.Controllers.Api;

[Route("api/accounts")]
[ApiController]
[Produces("application/json")]
public class AccountsController(IUsersService userService, IOptions<JwtSettings> jwtSettingsOption, IWebHostEnvironment env) : ControllerBase
{
    /// <summary>
    /// Retrieves a specific account by unique id
    /// </summary>
    /// <remarks>Retrieves the account details</remarks>
    /// <response code="200">Account details retrieved successfully</response>
    /// <response code="400">Account has missing/invalid values</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> Login(LoginRequest request, [FromQuery] int? withToken = null)
    {
        var email = request.Email;
        var password = request.Password;

        var user = await userService.GetUserByEmail(email);

        if (user == null)
        {
            var errorResponse = new ErrorResponse
            {
                Message = "Invalid email or password"
            };
            return BadRequest(errorResponse);
        }

        var hashedPassword = UsersHelper.HashPassword(password, user.Salt);

        if (user.PasswordHash != hashedPassword)
        {
            var errorResponse = new ErrorResponse
            {
                Message = "Invalid email or password"
            };
            return BadRequest(errorResponse);
        }

        var jwtSettings = jwtSettingsOption.Value;
        var token = JwtHelper.GenerateJwtToken(user, jwtSettings);

        user.LastLogin = DateTime.UtcNow;
        await userService.UpdateUser(user);

        Response.Cookies.Append(jwtSettings.CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromMinutes(jwtSettings.AccessTokenExpirationMinutes)
        });

        var response = new LoginResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email
        };

        if (withToken != null && withToken == 1 && env.IsDevelopment())
        {
            response.Token = token;
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves the currently logged in user
    /// </summary>
    /// <remarks>Returns a response containing the user's details</remarks>
    /// <response code="200">User details retrieved successfully</response>
    [HttpPost("getUser")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    public async Task<IActionResult> GetUser()
    {
        var currentUser = User.GetCurrentUser();
        Guard.Against.Null(currentUser);

        var user = await userService.GetUserByEmail(currentUser.Email);
        Guard.Against.Null(user);

        return Ok(new UserResponse { Id = user.Id.ToString(), Email = user.Email });
    }

    /// <summary>
    /// Logs out the user
    /// </summary>
    /// <remarks>Deletes the JWT token from the cookies</remarks>
    /// <response code="200">User logged out</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
    public IActionResult Logout()
    {
        var jwtSettings = jwtSettingsOption.Value;
        Response.Cookies.Delete(jwtSettings.CookieName);

        return Ok(new SuccessResponse { Message = "You are logged out!", ShowNotification = false });
    }

    /// <summary>
    /// Changes the password of the user
    /// </summary>
    /// <remarks>Requires the current password for verification</remarks>
    /// <response code="200">Password updated</response>
    /// <response code="400">Invalid current password</response>
    [HttpPost("changePassword")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var currentUser = User.GetCurrentUser();
        Guard.Against.Null(currentUser);

        var user = await userService.GetUserByEmail(currentUser.Email);
        Guard.Against.Null(user);

        var hashedCurrentPassword = UsersHelper.HashPassword(request.CurrentPassword, user.Salt);

        if (user.PasswordHash != hashedCurrentPassword)
        {
            var errorResponse = new ErrorResponse
            {
                Message = "Invalid current password",
                Field = "CurrentPassword"
            };
            return BadRequest(errorResponse);
        }

        var hashedNewPassword = UsersHelper.HashPassword(request.NewPassword, user.Salt);
        user.PasswordHash = hashedNewPassword;
        await userService.UpdateUser(user);

        return Ok(new SuccessResponse { Message = "Password is changed!" });
    }
}
