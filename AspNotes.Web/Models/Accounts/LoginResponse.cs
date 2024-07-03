using System.Text.Json.Serialization;

namespace AspNotes.Web.Models.Accounts;

public class LoginResponse
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }
}
