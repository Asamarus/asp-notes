using AspNotes.Entities;
using AspNotes.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNotes.Helpers;

/// <summary>
/// Provides helper methods for generating JWT tokens.
/// </summary>
public static class JwtHelper
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <param name="jwtSettings">The JWT settings to use for token generation.</param>
    /// <returns>A JWT token as a string.</returns>
    public static string GenerateJwtToken(UserEntity user, JwtSettings jwtSettings)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings.ValidIssuer,
            audience: jwtSettings.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
