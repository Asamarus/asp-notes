using AspNotes.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Tests.Common.Validation;

public class PositiveLongListAttributeTests
{
    private readonly PositiveLongListAttribute attribute = new();

    [Fact]
    public void IsValid_ShouldReturnSuccess_ForValidList()
    {
        // Arrange
        var validList = new List<long> { 1, 2, 3, 4, 5 };

        // Act
        var result = attribute.GetValidationResult(validList, new ValidationContext(new object()));

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_ShouldReturnValidationResult_ForListWithNegativeNumbers()
    {
        // Arrange
        var invalidList = new List<long> { 1, -2, 3, -4, 5 };

        // Act
        var result = attribute.GetValidationResult(invalidList, new ValidationContext(new object()));

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.NotNull(result);
        Assert.Equal("All numbers must be positive.", result.ErrorMessage);
    }

    [Fact]
    public void IsValid_ShouldReturnValidationResult_ForListWithZero()
    {
        // Arrange
        var invalidList = new List<long> { 1, 0, 3, 4, 5 };

        // Act
        var result = attribute.GetValidationResult(invalidList, new ValidationContext(new object()));

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.NotNull(result);
        Assert.Equal("All numbers must be positive.", result.ErrorMessage);
    }

    [Fact]
    public void IsValid_ShouldReturnValidationResult_ForInvalidDataType()
    {
        // Arrange
        var invalidData = "invalid data type";

        // Act
        var result = attribute.GetValidationResult(invalidData, new ValidationContext(new object()));

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.NotNull(result);
        Assert.Equal("Invalid data type. Expected a list of long integers.", result.ErrorMessage);
    }
}
