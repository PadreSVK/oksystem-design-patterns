// =============================================================================
// Hands-On 007 — Task 1: Fluent QueryBuilder<T>  (starter)
//
// The Query<T> record is provided — your task is to implement QueryBuilder<T>.
//
// Requirements:
//   • Where(predicate)   — adds a filter; returns this for chaining
//   • OrderBy(selector, descending = false) — sets the sort key; returns this
//   • Take(count)        — limits result count; returns this
//   • Skip(offset)       — sets the page offset; returns this
//   • Build()            — constructs and returns an immutable Query<T>
//                          throws InvalidOperationException if Take > 0 && Skip < 0
// =============================================================================

using System.Linq.Expressions;

namespace HandsOn.BuilderPrototypeSingleton;

// ── Product (provided — do not change) ───────────────────────────────────────

public sealed record Query<T>(
    IReadOnlyList<Expression<Func<T, bool>>> Filters,
    Expression<Func<T, object?>>?            OrderBy,
    bool                                     Descending,
    int?                                     Take,
    int                                      Skip);

// ── Builder (your task) ───────────────────────────────────────────────────────

public sealed class QueryBuilder<T>
{
    // TODO: add private backing fields for filters, orderBy, descending, take, skip

    public QueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        // TODO: add predicate to filters list; return this
        throw new NotImplementedException();
    }

    public QueryBuilder<T> OrderBy(Expression<Func<T, object?>> keySelector, bool descending = false)
    {
        // TODO: store keySelector and descending flag; return this
        throw new NotImplementedException();
    }

    public QueryBuilder<T> Take(int count)
    {
        // TODO: store count; return this
        throw new NotImplementedException();
    }

    public QueryBuilder<T> Skip(int offset)
    {
        // TODO: store offset; return this
        throw new NotImplementedException();
    }

    public Query<T> Build()
    {
        // TODO: validate — if _take > 0 && _skip < 0 throw InvalidOperationException
        // TODO: return new Query<T>(...) with all accumulated state
        throw new NotImplementedException();
    }
}
