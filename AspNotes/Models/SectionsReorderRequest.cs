using AspNotes.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a sections reorder request.
/// </summary>
public sealed class SectionsReorderRequest
{
    /// <summary>
    /// Gets or sets the list of section IDs in the desired order.
    /// </summary>
    [Required]
    [PositiveLongList(ErrorMessage = "All Ids must be positive numbers.")]
    public required List<long> Ids { get; set; }
}