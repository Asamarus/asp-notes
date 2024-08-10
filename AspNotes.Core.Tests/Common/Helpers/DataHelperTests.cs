using AspNotes.Core.Common.Helpers;

namespace AspNotes.Core.Tests.Common.Helpers;

public class DataHelperTests
{
    [Fact]
    public void GetComparer_ListsAreEqual_ReturnsTrue()
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
    public void GetComparer_ListsAreDifferent_ReturnsFalse()
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
    public void GetComparer_ListAndNull_ReturnsFalse()
    {
        // Arrange
        var comparer = DataHelper.GetComparer<int>();
        var list = new List<int> { 1, 2, 3 };

        // Act
        var areEqual = comparer.Equals(list, null);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void GetComparer_HashCodeForIdenticalLists_Match()
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
    public void GetComparer_SnapshotGeneratesEqualList_ReturnsTrue()
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
