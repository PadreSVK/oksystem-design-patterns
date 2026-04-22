// =============================================================================
// Hands-On 010 — Task 1: Pluggable Discount Engine with Strategy  (starter)
//
// Scenario: an e-commerce platform applies different pricing rules depending on
//           context (seasonal sales, bulk orders, no discount).  A future rule
//           must be addable without touching existing code — no switch/if chains.
//
// Your task:
//   Implement the three concrete discount strategies below so that
//   PriceCalculator produces the correct totals.
//
// Strategy pattern reminder:
//   • IDiscountStrategy = Strategy interface
//   • NoDiscount / PercentageDiscount / BulkDiscount = Concrete Strategies
//   • PriceCalculator  = Context — holds and delegates to the current strategy
// =============================================================================

namespace HandsOn.StrategyObserverCommand;

// ─── Strategy interface ───────────────────────────────────────────────────────

public interface IDiscountStrategy
{
    /// <summary>Returns the discounted unit price for the given quantity.</summary>
    decimal Apply(decimal unitPrice, int quantity);
}

// ─── Concrete strategies (your task) ─────────────────────────────────────────

/// <summary>No discount — always returns the original unit price.</summary>
public sealed class NoDiscount : IDiscountStrategy
{
    public decimal Apply(decimal unitPrice, int quantity)
    {
        // TODO: return unitPrice unchanged
        throw new NotImplementedException();
    }
}

/// <summary>Reduces the unit price by a fixed percentage (e.g. 0.10 = 10 % off).</summary>
public sealed class PercentageDiscount(decimal percent) : IDiscountStrategy
{
    private readonly decimal _percent = percent;

    public decimal Apply(decimal unitPrice, int quantity)
    {
        // TODO: return unitPrice * (1 - _percent)
        throw new NotImplementedException();
    }
}

/// <summary>
/// Applies a discounted unit price once the order reaches the threshold quantity.
/// Below the threshold the full price is charged.
/// </summary>
public sealed class BulkDiscount(int threshold, decimal discountedUnitPrice) : IDiscountStrategy
{
    private readonly int _threshold = threshold;
    private readonly decimal _discountedUnitPrice = discountedUnitPrice;

    public decimal Apply(decimal unitPrice, int quantity)
    {
        // TODO: if quantity >= _threshold return _discountedUnitPrice; else return unitPrice
        throw new NotImplementedException();
    }
}

// ─── Context (provided — do not modify) ──────────────────────────────────────

public sealed class PriceCalculator(IDiscountStrategy strategy)
{
    private IDiscountStrategy _strategy = strategy;

    /// <summary>Swaps the active strategy at runtime.</summary>
    public void SetStrategy(IDiscountStrategy strategy) => _strategy = strategy;

    /// <summary>Returns the total price for the given quantity at the discounted rate.</summary>
    public decimal Calculate(decimal unitPrice, int quantity)
        => _strategy.Apply(unitPrice, quantity) * quantity;
}
