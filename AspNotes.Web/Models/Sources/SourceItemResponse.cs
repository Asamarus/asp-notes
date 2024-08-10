using AspNotes.Core.Note.Models;

namespace AspNotes.Web.Models.Sources;

public class SourceItemResponse
{
    public string Id { get; set; } = null!;

    public string Link { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public bool ShowImage { get; set; } = false;

    public SourceItemResponse() { }

    public SourceItemResponse(NoteSource noteSource)
    {
        Id = noteSource.Id;
        Link = noteSource.Link;
        Title = noteSource.Title;
        Description = noteSource.Description;
        Image = noteSource.Image;
        ShowImage = noteSource.ShowImage;
    }
}
