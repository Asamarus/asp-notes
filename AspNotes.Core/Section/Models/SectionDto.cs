namespace AspNotes.Core.Section.Models;
public class SectionDto
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string Color { get; set; } = null!;

    public uint Position { get; set; }
}
