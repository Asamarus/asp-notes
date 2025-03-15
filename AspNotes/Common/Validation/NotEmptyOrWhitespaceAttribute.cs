using System.ComponentModel.DataAnnotations;

namespace AspNotes.Common.Validation;

/// <summary>
/// Validation attribute to ensure a string property is not empty or whitespace.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class NotEmptyOrWhitespaceAttribute : ValidationAttribute
{
    /// <summary>
    /// Validates whether the specified value is a non-empty, non-whitespace string.
    /// </summary>
    /// <param name="value">The value of the object to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>
    /// A <see cref="ValidationResult"/> that indicates whether the specified value is valid.
    /// </returns>
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            return ValidationResult.Success!;
        }

        return new ValidationResult("The field must not be empty or whitespace.");
    }
}
