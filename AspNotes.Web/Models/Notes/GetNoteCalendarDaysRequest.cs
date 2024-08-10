using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Notes;

public class GetNoteCalendarDaysRequest
{
    [Required]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12!")]
    public int Month { get; set; }

    [Required]
    [Range(1, 9999, ErrorMessage = "Year must be between 1 and 9999!")]
    public int Year { get; set; }

    public string? Section { get; set; }
}