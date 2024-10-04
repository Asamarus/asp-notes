namespace AspNotes.Web.Models.Sections;

public class SectionsResponse
{
    public string? Message { get; set; }

    public bool ShowNotification { get; set; } = false;

    public required List<SectionItemResponse> Sections { get; set; } = [];
}
