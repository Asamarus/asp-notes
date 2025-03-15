using AspNotes.Entities;
using AspNotes.Helpers;
using AspNotes.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNotes.Tests.Helpers;

public class JwtHelperTests
{
    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        // Arrange
        var user = new UserEntity
        {
            Id = 1,
            Email = "test@example.com"
        };

        var jwtSettings = new JwtSettings
        {
            Secret = "supersecretkey12345678901234567890",
            ValidIssuer = "testIssuer",
            ValidAudience = "testAudience",
            AccessTokenExpirationMinutes = 60,
            CookieName = "jwtCookie"
        };

        // Act
        var token = JwtHelper.GenerateJwtToken(user, jwtSettings);

        // Assert
        Assert.NotNull(token);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.ValidIssuer,
            ValidAudience = jwtSettings.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

        Assert.NotNull(validatedToken);
        Assert.Equal(user.Email, principal.FindFirst(ClaimTypes.Email)?.Value);
        Assert.Equal(user.Email, principal.FindFirst(ClaimTypes.Name)?.Value);
        Assert.Equal(user.Id.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }
}
