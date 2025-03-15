using AspNotes.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Tests.Common.Validation;

public class NotEmptyOrWhitespaceAttributeTests
{
    private readonly NotEmptyOrWhitespaceAttribute attribute = new();

    [Theory]
    [InlineData("ValidString")]
    [InlineData(" AnotherValidString ")]
    public void IsValid_ShouldReturnSuccess_ForNonEmptyNonWhitespaceStrings(string value)
    {
        // Act
        var result = attribute.GetValidationResult(value, new ValidationContext(new object()));

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void IsValid_ShouldReturnValidationResult_ForEmptyOrWhitespaceStrings(string value)
    {
        // Act
        var result = attribute.GetValidationResult(value, new ValidationContext(new object()));

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.NotNull(result);
        Assert.Equal("The field must not be empty or whitespace.", result.ErrorMessage);
    }

    [Fact]
    public void IsValid_ShouldReturnValidationResult_ForNullValue()
    {
        // Act
        var result = attribute.GetValidationResult(null, new ValidationContext(new object()));

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.NotNull(result);
        Assert.Equal("The field must not be empty or whitespace.", result.ErrorMessage);
    }
}
