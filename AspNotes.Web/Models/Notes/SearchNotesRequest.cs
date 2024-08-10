using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Notes;

public class SearchNotesRequest
{
    public string? Section { get; set; }

    [MinLength(3, ErrorMessage = "SearchTerm must be at least three symbols long.")]
    public string? SearchTerm { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be a positive integer larger or equal to 1.")]
    public int Page { get; set; } = 1;

    public string? Book { get; set; }

    public List<string> Tags { get; set; } = [];

    public bool InRandomOrder { get; set; } = false;

    public bool WithoutBook { get; set; } = false;

    public bool WithoutTags { get; set; } = false;

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }
}