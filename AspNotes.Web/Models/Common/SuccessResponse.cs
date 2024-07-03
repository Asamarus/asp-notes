namespace AspNotes.Web.Models.Common;

public class SuccessResponse
{
    public string Message { get; set; } = null!;

    public bool ShowNotification { get; set; } = true;
}
