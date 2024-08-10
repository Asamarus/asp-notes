using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Notes;

public class CreateNoteRequest
{
    [Required]
    public string Section { get; set; } = null!;

    public string? Book { get; set; }
}