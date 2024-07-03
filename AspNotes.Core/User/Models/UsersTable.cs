using AspNotes.Core.Common.Models;

namespace AspNotes.Core.User.Models;

public class UsersTable : DbTable
{
    public override string TableName => "Users";

    public string Id { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public string LastLogin { get; private set; }

    public string Salt { get; private set; }

    public UsersTable(string? alias = null) : base(alias)
    {
        alias ??= TableName;

        TableAlias = alias;

        Id = alias + "." + nameof(UserEntity.Id);

        Email = alias + "." + nameof(UserEntity.Email);

        PasswordHash = alias + "." + nameof(UserEntity.PasswordHash);

        LastLogin = alias + "." + nameof(UserEntity.LastLogin);

        Salt = alias + "." + nameof(UserEntity.Salt);
    }
}
