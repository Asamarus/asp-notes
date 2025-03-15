using AspNotes.Helpers;

namespace AspNotes.Tests.Helpers;

public class DataHelperTests
{
    [Fact]
    public void GetComparer_ShouldReturnTrueForEqualLists()
    {
        // Arrange
        var comparer = DataHelper.GetComparer<int>();
        var list1 = new List<int> { 1, 2, 3 };
        var list2 = new List<int> { 1, 2, 3 };

        // Act
        var areEqual = comparer.Equals(list1, list2);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void GetComparer_ShouldReturnFalseForDifferentLists()
    {
        // Arrange
        var comparer = DataHelper.GetComparer<int>();
        var list1 = new List<int> { 1, 2, 3 };
        var list2 = new List<int> { 4, 5, 6 };

        // Act
        var areEqual = comparer.Equals(list1, list2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void GetComparer_ShouldReturnMatchingHashCodesForIdenticalLists()
    {
        // Arrange
        var comparer = DataHelper.GetComparer<int>();
        var list1 = new List<int> { 1, 2, 3 };
        var list2 = new List<int> { 1, 2, 3 };

        // Act
        var hash1 = comparer.GetHashCode(list1);
        var hash2 = comparer.GetHashCode(list2);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetComparer_ShouldReturnEqualListForSnapshot()
    {
        // Arrange
        var comparer = DataHelper.GetComparer<int>();
        var list = new List<int> { 1, 2, 3 };

        // Act
        var snapshot = comparer.Snapshot(list);

        // Assert
        Assert.Equal(list, snapshot);
    }
}
