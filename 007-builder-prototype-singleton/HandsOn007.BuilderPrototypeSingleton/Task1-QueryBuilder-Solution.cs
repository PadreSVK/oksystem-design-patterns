// =============================================================================
// Hands-On 007 — Task 1: Fluent QueryBuilder<T>  (SOLUTION)
// =============================================================================

using System.Linq.Expressions;

namespace HandsOn.BuilderPrototypeSingleton;

public sealed record Query<T>(
    IReadOnlyList<Expression<Func<T, bool>>> Filters,
    Expression<Func<T, object?>>?            OrderBy,
    bool                                     Descending,
    int?                                     Take,
    int                                      Skip);

public sealed class QueryBuilder<T>
{
    private readonly List<Expression<Func<T, bool>>> _filters = [];
    private Expression<Func<T, object?>>? _orderBy;
    private bool _descending;
    private int? _take;
    private int  _skip;

    public QueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        _filters.Add(predicate);
        return this;
    }

    public QueryBuilder<T> OrderBy(Expression<Func<T, object?>> keySelector, bool descending = false)
    {
        _orderBy     = keySelector;
        _descending  = descending;
        return this;
    }

    public QueryBuilder<T> Take(int count)  { _take = count;  return this; }
    public QueryBuilder<T> Skip(int offset) { _skip = offset; return this; }

    public Query<T> Build()
    {
        if (_take > 0 && _skip < 0)
            throw new InvalidOperationException(
                $"Skip must be >= 0 when Take is set (Take={_take}, Skip={_skip}).");

        return new Query<T>(_filters.AsReadOnly(), _orderBy, _descending, _take, _skip);
    }
}
