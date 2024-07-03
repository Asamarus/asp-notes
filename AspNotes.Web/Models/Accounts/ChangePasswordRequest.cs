using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Accounts;

public class ChangePasswordRequest
{
    [Required]
    [MinLength(6, ErrorMessage = "Minimum password length is 6")]
    public string CurrentPassword { get; set; } = null!;

    [Required]
    [MinLength(6, ErrorMessage = "Minimum password length is 6")]
    public string NewPassword { get; set; } = null!;

    [Required]
    [MinLength(6, ErrorMessage = "Minimum password length is 6")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
    public string PasswordRepeat { get; set; } = null!;
}
