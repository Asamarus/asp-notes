namespace AspNotes.Core.Common.Models;

public abstract class EntityBase
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}