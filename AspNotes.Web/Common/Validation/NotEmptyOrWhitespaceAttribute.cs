using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Common.Validation;

public class NotEmptyOrWhitespaceAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            return ValidationResult.Success!;
        }
        return new ValidationResult("The field must not be empty or whitespace.");
    }
}