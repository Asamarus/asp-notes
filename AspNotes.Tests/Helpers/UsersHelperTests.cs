using AspNotes.Helpers;

namespace AspNotes.Tests.Helpers;

public class UsersHelperTests
{
    [Fact]
    public void GenerateSalt_ShouldReturnNonEmptySalt()
    {
        // Act
        var salt = UsersHelper.GenerateSalt();

        // Assert
        Assert.NotNull(salt);
        Assert.Equal(128 / 8, salt.Length); // Ensure the salt length is correct
    }

    [Fact]
    public void HashPassword_ShouldReturnConsistentHashForSameInput()
    {
        // Arrange
        var password = "testPassword";
        var salt = UsersHelper.GenerateSalt();

        // Act
        var hash1 = UsersHelper.HashPassword(password, salt);
        var hash2 = UsersHelper.HashPassword(password, salt);

        // Assert
        Assert.Equal(hash1, hash2); // Ensure the hash is consistent for the same input
    }

    [Fact]
    public void HashPassword_ShouldReturnDifferentHashForDifferentInput()
    {
        // Arrange
        var password1 = "testPassword1";
        var password2 = "testPassword2";
        var salt = UsersHelper.GenerateSalt();

        // Act
        var hash1 = UsersHelper.HashPassword(password1, salt);
        var hash2 = UsersHelper.HashPassword(password2, salt);

        // Assert
        Assert.NotEqual(hash1, hash2); // Ensure the hash is different for different input
    }

    [Fact]
    public void HashPassword_ShouldReturnDifferentHashForDifferentSalt()
    {
        // Arrange
        var password = "testPassword";
        var salt1 = UsersHelper.GenerateSalt();
        var salt2 = UsersHelper.GenerateSalt();

        // Act
        var hash1 = UsersHelper.HashPassword(password, salt1);
        var hash2 = UsersHelper.HashPassword(password, salt2);

        // Assert
        Assert.NotEqual(hash1, hash2); // Ensure the hash is different for different salt
    }
}
