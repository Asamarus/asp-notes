namespace AspNotes.Web.Models.Sources;

public class SourcesResponse
{
    public string? Message { get; set; }

    public bool ShowNotification { get; set; } = false;

    public required List<SourceItemResponse> Sources { get; set; } = [];
}
