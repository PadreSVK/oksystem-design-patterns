// =============================================================================
// Hands-On 007 — Task 1 Tests: QueryBuilder<T>
//
// These tests are always compiled and run.
// Fix Task1-QueryBuilder.cs in HandsOn007.BuilderPrototypeSingleton/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.BuilderPrototypeSingleton;

namespace HandsOn007.Tests;

public class QueryBuilderTests
{
    private sealed record Order(bool IsActive, int CustomerId, DateTime CreatedAt);

    [Fact]
    public void Build_WithFiltersAndPaging_ReturnsCorrectQuery()
    {
        var query = new QueryBuilder<Order>()
            .Where(o => o.IsActive)
            .Where(o => o.CustomerId == 42)
            .OrderBy(o => o.CreatedAt, descending: true)
            .Skip(20).Take(10)
            .Build();

        Assert.Equal(2, query.Filters.Count);
        Assert.NotNull(query.OrderBy);
        Assert.True(query.Descending);
        Assert.Equal(10, query.Take);
        Assert.Equal(20, query.Skip);
    }

    [Fact]
    public void Build_NoFiltersNoOrdering_ReturnsEmptyQuery()
    {
        var query = new QueryBuilder<Order>().Build();

        Assert.Empty(query.Filters);
        Assert.Null(query.OrderBy);
        Assert.Null(query.Take);
        Assert.Equal(0, query.Skip);
    }

    [Fact]
    public void Build_NegativeSkipWithTake_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new QueryBuilder<Order>().Take(10).Skip(-1).Build());
    }

    [Fact]
    public void Build_ZeroSkipWithTake_DoesNotThrow()
    {
        var query = new QueryBuilder<Order>().Take(10).Skip(0).Build();
        Assert.Equal(10, query.Take);
    }

    [Fact]
    public void Build_NegativeSkipWithoutTake_DoesNotThrow()
    {
        // Skip < 0 is only invalid when Take is also set
        var query = new QueryBuilder<Order>().Skip(-5).Build();
        Assert.Equal(-5, query.Skip);
    }
}
