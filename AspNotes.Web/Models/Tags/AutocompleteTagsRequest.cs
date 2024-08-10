using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Tags;

public class AutocompleteTagsRequest
{
    [Required]
    [MinLength(2, ErrorMessage = "Search term must be at least 2 characters long.")]
    public string SearchTerm { get; set; } = null!;

    public string? Section { get; set; }
}