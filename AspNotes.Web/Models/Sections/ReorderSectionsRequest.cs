using AspNotes.Web.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Sections;

public class ReorderSectionsRequest
{
    [Required]
    [PositiveLongList(ErrorMessage = "All Ids must be positive numbers!")]
    public List<long> Ids { get; set; } = null!;
}