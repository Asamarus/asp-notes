using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Accounts;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}
