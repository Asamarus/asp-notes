using System.Text.Json.Serialization;

namespace AspNotes.Web.Models.Common;

public class ErrorResponse
{
    public string Message { get; set; } = null!;

    public bool ShowNotification { get; set; } = true;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Field { get; set; }
}
