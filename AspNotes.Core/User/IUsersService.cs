using AspNotes.Core.User.Models;
using System.Linq.Expressions;

namespace AspNotes.Core.User;

public interface IUsersService
{
    Task<UserEntity?> GetUserById(long id);
    Task<UserEntity?> GetUserByEmail(string email);
    Task<List<UserEntity>> GetFilteredUsers(Expression<Func<UserEntity, bool>> predicate);
    Task<List<UserEntity>> GetAllUsers();
    Task<UserEntity> CreateUser(string email, string password);
    Task UpdateUser(UserEntity user);
    Task DeleteUser(UserEntity user);
}
