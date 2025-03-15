using System.ComponentModel.DataAnnotations;

namespace AspNotes.Common.Validation;

/// <summary>
/// Validates that a list contains only positive long integers.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class PositiveLongListAttribute : ValidationAttribute
{
    /// <summary>
    /// Determines whether the specified value of the object is valid.
    /// </summary>
    /// <param name="value">The value of the object to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A <see cref="ValidationResult"/> instance. Success if the list contains only positive long integers; otherwise, an error message.</returns>
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is List<long> longList)
        {
            foreach (var number in longList)
            {
                if (number <= 0)
                {
                    return new ValidationResult(ErrorMessage ?? "All numbers must be positive.");
                }
            }

            return ValidationResult.Success!;
        }

        return new ValidationResult("Invalid data type. Expected a list of long integers.");
    }
}