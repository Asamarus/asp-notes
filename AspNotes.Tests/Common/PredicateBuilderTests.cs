using AspNotes.Common;
using System.Linq.Expressions;

namespace AspNotes.Tests.Common;

public class PredicateBuilderTests
{
    [Fact]
    public void True_ShouldReturnPredicateThatIsAlwaysTrue()
    {
        // Arrange
        var predicate = PredicateBuilder.True<int>();

        // Act
        var result = predicate.Compile().Invoke(0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void False_ShouldReturnPredicateThatIsAlwaysFalse()
    {
        // Arrange
        var predicate = PredicateBuilder.False<int>();

        // Act
        var result = predicate.Compile().Invoke(0);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Or_ShouldCombinePredicatesUsingLogicalOr()
    {
        // Arrange
        Expression<Func<int, bool>> predicate1 = x => x < 10;
        Expression<Func<int, bool>> predicate2 = x => x > 15;

        // Act
        var combinedPredicate = predicate1.Or(predicate2);
        var compiledPredicate = combinedPredicate.Compile();

        // Assert
        Assert.True(compiledPredicate.Invoke(4));    // 4 < 10
        Assert.True(compiledPredicate.Invoke(6));    // 6 < 10
        Assert.False(compiledPredicate.Invoke(10));  // 10 >= 10 and 10 <= 15
        Assert.False(compiledPredicate.Invoke(11));  // 11 is between 10 and 15
        Assert.True(compiledPredicate.Invoke(16));   // 16 > 15
    }
}
