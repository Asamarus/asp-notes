namespace AspNotes.Web.Models.Sources;

public class SourcesResponse
{
    public string? Message { get; set; }

    public required List<SourceItemResponse> Sources { get; set; } = [];
}
