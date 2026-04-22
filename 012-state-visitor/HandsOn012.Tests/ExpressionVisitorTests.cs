// =============================================================================
// Hands-On 012 — Task 3 Tests: ExpressionVisitor Rewriting
//
// These tests are always compiled and run.
// Fix Task3-ExpressionVisitor.cs in HandsOn012.StateVisitor/ until all pass.
// =============================================================================

using System.Linq.Expressions;
using Xunit;
using HandsOn.StateVisitor;

namespace HandsOn012.Tests;

public class ExpressionVisitorTests
{
    private sealed record Product(string Name, int Quantity);

    private static Expression<Func<Product, bool>> Rewrite(Expression<Func<Product, bool>> expr)
        => (Expression<Func<Product, bool>>)new CaseInsensitiveStringVisitor().Visit(expr);

    // ── Basic rewriting ───────────────────────────────────────────────────────

    [Fact]
    public void Rewritten_MatchesUppercaseInput()
    {
        Expression<Func<Product, bool>> pred = p => p.Name == "widget";
        var rewritten = Rewrite(pred);
        var compiled  = rewritten.Compile();

        Assert.True(compiled(new Product("WIDGET", 1)));
    }

    [Fact]
    public void Rewritten_MatchesMixedCaseInput()
    {
        Expression<Func<Product, bool>> pred = p => p.Name == "widget";
        var rewritten = Rewrite(pred);
        var compiled  = rewritten.Compile();

        Assert.True(compiled(new Product("Widget", 1)));
    }

    [Fact]
    public void Rewritten_MatchesExactCaseInput()
    {
        Expression<Func<Product, bool>> pred = p => p.Name == "widget";
        var rewritten = Rewrite(pred);
        var compiled  = rewritten.Compile();

        Assert.True(compiled(new Product("widget", 1)));
    }

    [Fact]
    public void Rewritten_DoesNotMatchDifferentName()
    {
        Expression<Func<Product, bool>> pred = p => p.Name == "widget";
        var rewritten = Rewrite(pred);
        var compiled  = rewritten.Compile();

        Assert.False(compiled(new Product("gadget", 1)));
    }

    // ── Original predicate still uses case-sensitive equality ─────────────────

    [Fact]
    public void Original_DoesNotMatchUppercaseInput()
    {
        Expression<Func<Product, bool>> pred = p => p.Name == "widget";
        var compiled = pred.Compile();

        // Rewriting must not mutate the original expression
        Assert.False(compiled(new Product("WIDGET", 1)));
    }

    // ── Non-string == comparisons are left untouched ──────────────────────────

    [Fact]
    public void Rewritten_IntEquality_StillWorks()
    {
        Expression<Func<Product, bool>> pred = p => p.Quantity == 5;
        var rewritten = Rewrite(pred);
        var compiled  = rewritten.Compile();

        Assert.True(compiled(new Product("x", 5)));
        Assert.False(compiled(new Product("x", 6)));
    }

    // ── Compiled delegate executes correctly end-to-end ──────────────────────

    [Fact]
    public void Rewritten_FilterList_ReturnsMatchingItems()
    {
        var products = new[]
        {
            new Product("Widget",  10),
            new Product("WIDGET",  20),
            new Product("Gadget",  5),
        };

        Expression<Func<Product, bool>> pred = p => p.Name == "widget";
        var compiled = Rewrite(pred).Compile();

        var matches = products.Where(compiled).ToList();

        Assert.Equal(2, matches.Count);
        Assert.DoesNotContain(matches, p => p.Name == "Gadget");
    }

    // ── Visitor returns a new expression (does not modify the original node) ──

    [Fact]
    public void Rewritten_IsNotSameReference_AsOriginal()
    {
        Expression<Func<Product, bool>> pred = p => p.Name == "widget";
        var rewritten = Rewrite(pred);

        Assert.NotSame(pred, rewritten);
    }
}
