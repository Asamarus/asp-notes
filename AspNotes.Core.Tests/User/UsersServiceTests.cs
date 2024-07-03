using AspNotes.Core.User;

namespace AspNotes.Core.Tests.User;

public class UsersServiceTests : DatabaseTestBase
{
    [Fact]
    public async Task GetUserById_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);

        var user = await userService.CreateUser("test1@gmail.com", "test password");

        // Act
        var foundUser = await userService.GetUserById(user.Id);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal("test1@gmail.com", foundUser.Email);
    }

    [Fact]
    public async Task GetUserByEmail_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);

        await userService.CreateUser("test1@gmail.com", "test password");

        // Act
        var foundUser = await userService.GetUserByEmail("test1@gmail.com");

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal("test1@gmail.com", foundUser.Email);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsAllUsers()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);

        await userService.CreateUser("test1@gmail.com", "test password");
        await userService.CreateUser("test2@gmail.com", "test password");
        await userService.CreateUser("test3@gmail.com", "test password");

        // Act
        var users = await userService.GetAllUsers();

        // Assert
        Assert.NotNull(users);
        Assert.True(users.Count > 0);
        Assert.Equal("user@mail.com", users.First().Email);
    }

    [Fact]
    public async Task CreateUser_CreatesAUser()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);

        // Act
        var user = await userService.CreateUser("test@gmail.com", "test password");

        // Assert
        Assert.NotNull(user);
        Assert.Equal("test@gmail.com", user.Email);
        Assert.NotNull(user.PasswordHash);
        Assert.NotNull(user.Salt);
        Assert.True(user.Id > 0);
        Assert.True(user.CreatedAt > DateTime.MinValue);
        Assert.True(user.UpdatedAt > DateTime.MinValue);
        Assert.Equal(user.PasswordHash, UsersHelper.HashPassword("test password", user.Salt));
    }

    [Fact]
    public async Task UpdateUser_UpdatesUser()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);

        var user = await userService.CreateUser("test@gmail.com", "test password");

        // Act
        user.Email = "test2@mail.com";
        user.LastLogin = DateTime.Now;
        await userService.UpdateUser(user);

        var updatedUser = await userService.GetUserById(user.Id);

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal("test2@mail.com", user.Email);
        Assert.True(updatedUser.LastLogin == user.LastLogin);
    }

    [Fact]
    public async Task DeleteUser_DeletesUser()
    {
        // Arrange
        var userService = new UsersService(DbFixture.DbContext);

        var user = await userService.CreateUser("test@gmail.com", "test password");

        // Act
        await userService.DeleteUser(user);

        var deletedUser = await userService.GetUserById(user.Id);

        // Assert
        Assert.Null(deletedUser);
    }
}
