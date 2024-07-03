using AspNotes.Core.User.Models;
using AspNotes.Web.Helpers;
using AspNotes.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AspNotes.Web.Tests.Helpers;

public class JwtHelperTests
{
    [Fact]
    public void GenerateJwtToken_ReturnsValidJwtToken()
    {
        // Arrange
        var user = new UserEntity { Email = "test@example.com", Id = 1 };
        var jwtSettings = new JwtSettings
        {
            Secret = "ThisIsASecretKeyForJwtThisIsASecretKeyForJwt",
            ValidIssuer = "TestIssuer",
            ValidAudience = "TestAudience",
            AccessTokenExpirationMinutes = 60
        };

        // Act
        var token = JwtHelper.GenerateJwtToken(user, jwtSettings);

        // Assert
        Assert.NotNull(token);

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(jwtSettings.ValidIssuer, jwtToken.Issuer);
        Assert.Equal(jwtSettings.ValidAudience, jwtToken.Audiences.First());
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal(user.Id.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }
}
