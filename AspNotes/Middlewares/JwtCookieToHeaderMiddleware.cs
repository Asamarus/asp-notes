using AspNotes.Models;
using Microsoft.Extensions.Options;

namespace AspNotes.Middlewares;

/// <summary>
/// Initializes a new instance of the <see cref="JwtCookieToHeaderMiddleware"/> class.
/// </summary>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="jwtSettingsOption">The JWT settings options.</param>
public class JwtCookieToHeaderMiddleware(
    RequestDelegate next,
    IOptions<JwtSettings> jwtSettingsOption)
{
    /// <summary>
    /// Invokes the middleware to transfer JWT from cookie to Authorization header.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task Invoke(HttpContext context)
    {
        var jwtSettings = jwtSettingsOption.Value;

        if (
            jwtSettings != null &&
            !string.IsNullOrWhiteSpace(jwtSettings.CookieName) &&
            context.Request.Cookies.TryGetValue(jwtSettings.CookieName, out var token) &&
            !string.IsNullOrWhiteSpace(token))
        {
            context.Request.Headers.Append("Authorization", $"Bearer {token}");
        }

        await next(context);
    }
}