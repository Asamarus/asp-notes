using AspNotes.Core.Section.Models;

namespace AspNotes.Web.Models.Sections;

public class SectionItemResponse
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string Color { get; set; } = null!;

    public SectionItemResponse() { }

    public SectionItemResponse(SectionDto section)
    {
        Id = section.Id;
        Name = section.Name;
        DisplayName = section.DisplayName;
        Color = section.Color;
    }
}
