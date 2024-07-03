using AspNotes.Core.User;

namespace AspNotes.Core.Tests.User;

public class UsersHelperTests
{
    [Fact]
    public void HashPassword_ReturnsDifferentHashes_ForDifferentPasswords()
    {
        // Arrange
        var salt = UsersHelper.GenerateSalt();

        // Act
        var hash1 = UsersHelper.HashPassword("password1", salt);
        var hash2 = UsersHelper.HashPassword("password2", salt);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_ReturnsSameHash_ForSamePasswordAndSalt()
    {
        // Arrange
        var salt = UsersHelper.GenerateSalt();

        // Act
        var hash1 = UsersHelper.HashPassword("password", salt);
        var hash2 = UsersHelper.HashPassword("password", salt);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GenerateSalt_ReturnsDifferentSalts_OnEachCall()
    {
        // Act
        var salt1 = UsersHelper.GenerateSalt();
        var salt2 = UsersHelper.GenerateSalt();

        // Assert
        Assert.NotEqual(salt1, salt2);
    }
}
