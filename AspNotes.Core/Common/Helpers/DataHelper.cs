using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspNotes.Core.Common.Helpers;

/// <summary>
/// Provides utility methods for data manipulation and comparison.
/// </summary>
public class DataHelper
{
    /// <summary>
    /// Creates a <see cref="ValueComparer{T}"/> for comparing lists of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <returns>A <see cref="ValueComparer{T}"/> configured to compare lists of type <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// This comparer checks for sequence equality, computes a combined hash code for the list,
    /// and can create a list from an enumerable of type <typeparamref name="T"/>.
    /// </remarks>
    public static ValueComparer<List<T>> GetComparer<T>()
    {
        return new ValueComparer<List<T>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c != null ? c.Where(e => e != null).Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())) : 0,
            c => (c ?? Enumerable.Empty<T>()).ToList());
    }
}
