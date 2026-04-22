// =============================================================================
// Hands-On 010 — Task 1 Tests: Pluggable Discount Engine (Strategy)
//
// These tests are always compiled and run.
// Fix Task1-Strategy.cs in HandsOn010.StrategyObserverCommand/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.StrategyObserverCommand;

namespace HandsOn010.Tests;

public class StrategyTests
{
    // ── NoDiscount ────────────────────────────────────────────────────────────

    [Fact]
    public void NoDiscount_ReturnsFullUnitPrice()
    {
        var strategy = new NoDiscount();
        Assert.Equal(50m, strategy.Apply(50m, 1));
    }

    [Fact]
    public void NoDiscount_IgnoresQuantity()
    {
        var strategy = new NoDiscount();
        Assert.Equal(50m, strategy.Apply(50m, 100));
    }

    // ── PercentageDiscount ────────────────────────────────────────────────────

    [Fact]
    public void PercentageDiscount_ReducesPriceByPercent()
    {
        var strategy = new PercentageDiscount(0.10m); // 10 % off
        Assert.Equal(90m, strategy.Apply(100m, 1));
    }

    [Fact]
    public void PercentageDiscount_ZeroPercent_ReturnsFullPrice()
    {
        var strategy = new PercentageDiscount(0m);
        Assert.Equal(200m, strategy.Apply(200m, 5));
    }

    [Fact]
    public void PercentageDiscount_HundredPercent_ReturnsFree()
    {
        var strategy = new PercentageDiscount(1m);
        Assert.Equal(0m, strategy.Apply(99m, 3));
    }

    // ── BulkDiscount ──────────────────────────────────────────────────────────

    [Fact]
    public void BulkDiscount_BelowThreshold_ReturnsFullPrice()
    {
        var strategy = new BulkDiscount(threshold: 10, discountedUnitPrice: 8m);
        Assert.Equal(9m, strategy.Apply(9m, 5)); // 5 < 10 → full price
    }

    [Fact]
    public void BulkDiscount_AtThreshold_ReturnsDiscountedPrice()
    {
        var strategy = new BulkDiscount(threshold: 10, discountedUnitPrice: 8m);
        Assert.Equal(8m, strategy.Apply(9m, 10)); // exactly 10 → discounted
    }

    [Fact]
    public void BulkDiscount_AboveThreshold_ReturnsDiscountedPrice()
    {
        var strategy = new BulkDiscount(threshold: 10, discountedUnitPrice: 8m);
        Assert.Equal(8m, strategy.Apply(9m, 50));
    }

    // ── PriceCalculator (context) ─────────────────────────────────────────────

    [Fact]
    public void PriceCalculator_MultipliesDiscountedPriceByQuantity()
    {
        var calc = new PriceCalculator(new PercentageDiscount(0.20m)); // 20 % off
        // 100 * 0.80 = 80 per unit × 3 = 240
        Assert.Equal(240m, calc.Calculate(100m, 3));
    }

    [Fact]
    public void PriceCalculator_StrategyCanBeSwappedAtRuntime()
    {
        var calc = new PriceCalculator(new NoDiscount());
        Assert.Equal(500m, calc.Calculate(100m, 5)); // no discount

        calc.SetStrategy(new PercentageDiscount(0.50m)); // 50 % off
        Assert.Equal(250m, calc.Calculate(100m, 5)); // same price, new strategy
    }
}
