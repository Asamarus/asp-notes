using AspNotes.Helpers;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mime;
using System.Security.Claims;

namespace AspNotes.Controllers;

/// <summary>
/// Controller for managing user-related actions.
/// </summary>
[Route("api/users")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class UsersController(
    IAppDbContext dbContext,
    IOptions<JwtSettings> jwtSettingsOption,
    IWebHostEnvironment env)
    : ControllerBase
{
    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="request">The login request containing email and password.</param>
    /// <param name="withToken">Whether to include the token in the response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns a JWT token if authentication is successful.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UsersLoginResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Login(
        [FromBody] UsersLoginRequest request,
        [FromQuery] int? withToken,
        CancellationToken cancellationToken)
    {
        var userEntity = await dbContext.Users
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (userEntity == null)
        {
            return ValidationProblem(title: "Invalid email or password!");
        }

        var hashedPassword = UsersHelper.HashPassword(request.Password, userEntity.Salt);

        if (userEntity.PasswordHash != hashedPassword)
        {
            return ValidationProblem(title: "Invalid email or password!");
        }

        var jwtSettings = jwtSettingsOption.Value;
        var token = JwtHelper.GenerateJwtToken(userEntity, jwtSettings);

        userEntity.LastLogin = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        Response.Cookies.Append(jwtSettings.CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromMinutes(jwtSettings.AccessTokenExpirationMinutes),
        });

        var response = new UsersLoginResponse
        {
            Id = userEntity.Id,
            Email = request.Email,
        };

        if (withToken != null && withToken == 1 && env.IsDevelopment())
        {
            response.AccessToken = token;
        }

        return Created(string.Empty, response);
    }

    /// <summary>
    /// Logs out the user.
    /// </summary>
    /// <remarks>Deletes the JWT token from the cookies.</remarks>
    /// <returns>Returns no content if the user is logged out successfully.</returns>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        var jwtSettings = jwtSettingsOption.Value;
        Response.Cookies.Delete(jwtSettings.CookieName);

        return NoContent();
    }

    /// <summary>
    /// Gets the current authenticated user's information.
    /// </summary>
    /// <returns>Returns the current user's information.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsersCurrentUserResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        return Ok(new UsersCurrentUserResponse
        {
            Id = userId,
            Email = email,
        });
    }

    /// <summary>
    /// Changes the password of the current authenticated user.
    /// </summary>
    /// <param name="request">The request containing current and new passwords.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns no content if the password is changed successfully.</returns>
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> ChangePassword(
        [FromBody] UsersChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var userEntity = await dbContext.Users.FindAsync(long.Parse(userId), cancellationToken);

        if (userEntity is null)
        {
            return NotFound();
        }

        var hashedCurrentPassword = UsersHelper.HashPassword(request.CurrentPassword, userEntity.Salt);

        if (userEntity.PasswordHash != hashedCurrentPassword)
        {
            return ValidationProblem(title: "Invalid current password!");
        }

        var hashedNewPassword = UsersHelper.HashPassword(request.NewPassword, userEntity.Salt);
        userEntity.PasswordHash = hashedNewPassword;
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
