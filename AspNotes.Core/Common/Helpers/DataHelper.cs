using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspNotes.Core.Common.Helpers;

public class DataHelper
{
    public static ValueComparer<List<T>> GetComparer<T>()
    {
        return new ValueComparer<List<T>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c != null ? c.Where(e => e != null).Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())) : 0,
            c => (c ?? Enumerable.Empty<T>()).ToList());
    }
}
