namespace AspNotes.Web.Models.Sections;

public class SectionsResponse
{
    public string? Message { get; set; }

    public required List<SectionItemResponse> Sections { get; set; } = [];
}
