using AspNotes.Web.Models;
using Microsoft.Extensions.Options;

namespace AspNotes.Web.Middlewares;

public class JwtCookieToHeaderMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettingsOption)
{
    public async Task Invoke(HttpContext context)
    {
        var jwtSettings = jwtSettingsOption.Value;

        if (
            jwtSettings != null &&
            !string.IsNullOrWhiteSpace(jwtSettings.CookieName) &&
            context.Request.Cookies.TryGetValue(jwtSettings.CookieName, out var token) &&
            !string.IsNullOrWhiteSpace(token)
        )
        {
            context.Request.Headers.Append("Authorization", $"Bearer {token}");
        }

        await next(context);
    }
}
