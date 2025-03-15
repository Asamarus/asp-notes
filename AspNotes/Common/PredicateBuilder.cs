using System.Linq.Expressions;

namespace AspNotes.Common;

/// <summary>
/// Provides methods to build predicates for LINQ queries.
/// </summary>
public static class PredicateBuilder
{
    /// <summary>
    /// Returns a predicate that is always true.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the predicate.</typeparam>
    /// <returns>An expression that represents a predicate that is always true.</returns>
    public static Expression<Func<T, bool>> True<T>() => f => true;

    /// <summary>
    /// Returns a predicate that is always false.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the predicate.</typeparam>
    /// <returns>An expression that represents a predicate that is always false.</returns>
    public static Expression<Func<T, bool>> False<T>() => f => false;

    /// <summary>
    /// Combines two predicates using a logical OR operation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the predicates.</typeparam>
    /// <param name="expr1">The first predicate.</param>
    /// <param name="expr2">The second predicate.</param>
    /// <returns>An expression that represents the logical OR of the two predicates.</returns>
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }
}
