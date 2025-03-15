using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to get calendar days for a specific month and year.
/// </summary>
public sealed class NotesGetCalendarDaysRequest
{
    /// <summary>
    /// Gets or sets the month for the calendar days request.
    /// </summary>
    [Required]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
    public int Month { get; set; }

    /// <summary>
    /// Gets or sets the year for the calendar days request.
    /// </summary>
    [Required]
    [Range(1, 9999, ErrorMessage = "Year must be between 1 and 9999.")]
    public int Year { get; set; }

    /// <summary>
    /// Gets or sets the section of the note.
    /// </summary>
    public string? Section { get; set; }
}