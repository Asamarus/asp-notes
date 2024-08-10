using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Notes;

public class UpdateNoteTagsRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number!")]
    public long Id { get; set; }

    [Required]
    public HashSet<string> Tags { get; set; } = [];
}