using AspNotes.Core.Common;
using AspNotes.Core.User.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AspNotes.Core.User;

public class UsersService(NotesDbContext context) : IUsersService
{
    public async Task<UserEntity?> GetUserById(long id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<UserEntity?> GetUserByEmail(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    //var usersWithGmail = await _userService.GetUsers(user => user.Email.EndsWith("@gmail.com"));
    public async Task<List<UserEntity>> GetFilteredUsers(Expression<Func<UserEntity, bool>> predicate)
    {
        return await context.Users.Where(predicate).AsNoTracking().ToListAsync();
    }

    public async Task<List<UserEntity>> GetAllUsers()
    {
        return await context.Users.AsNoTracking().ToListAsync();
    }

    public async Task<UserEntity> CreateUser(string email, string password)
    {
        var salt = UsersHelper.GenerateSalt();
        var passwordHash = UsersHelper.HashPassword(password, salt);

        var user = new UserEntity
        {
            Email = email,
            PasswordHash = passwordHash,
            Salt = salt
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUser(UserEntity user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUser(UserEntity user)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}
