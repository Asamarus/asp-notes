using AspNotes.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Core.User.Models;

[Index(nameof(Email), IsUnique = true)]
public class UserEntity : EntityBase
{
    [Key]
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? LastLogin { get; set; }

    public byte[] Salt { get; set; } = null!;
}
