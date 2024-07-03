using AspNotes.Web.Helpers;
using System.Security.Claims;

namespace AspNotes.Web.Tests.Helpers;
public class CurrentUserHelperTests
{
    [Fact]
    public void GetCurrentUser_ReturnsCurrentUser()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act
        var currentUser = CurrentUserHelper.GetCurrentUser(claimsPrincipal);

        // Assert
        Assert.NotNull(currentUser);
        Assert.Equal(1, currentUser.Id);
        Assert.Equal("test@example.com", currentUser.Email);
    }

    [Fact]
    public void GetCurrentUser_ThrowsException_WhenClaimsAreMissing()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        Assert.Throws<Exception>(() => CurrentUserHelper.GetCurrentUser(claimsPrincipal));
    }

    [Fact]
    public void GetCurrentUser_ThrowsException_WhenIdIsNotInteger()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "not an integer"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        Assert.Throws<Exception>(() => CurrentUserHelper.GetCurrentUser(claimsPrincipal));
    }
}
