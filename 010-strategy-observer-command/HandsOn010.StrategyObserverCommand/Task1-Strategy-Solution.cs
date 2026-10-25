// =============================================================================
// Hands-On 010 — Task 1: Pluggable Discount Engine with Strategy  (solution)
// =============================================================================

namespace HandsOn.StrategyObserverCommand;

public interface IDiscountStrategy
{
    decimal Apply(decimal unitPrice, int quantity);
}

public sealed class NoDiscount : IDiscountStrategy
{
    public decimal Apply(decimal unitPrice, int quantity) => unitPrice;
}

public sealed class PercentageDiscount(decimal percent) : IDiscountStrategy
{
    public decimal Apply(decimal unitPrice, int quantity) => unitPrice * (1 - percent);
}

public sealed class BulkDiscount(int threshold, decimal discountedUnitPrice) : IDiscountStrategy
{
    public decimal Apply(decimal unitPrice, int quantity)
        => quantity >= threshold ? discountedUnitPrice : unitPrice;
}

public sealed class PriceCalculator(IDiscountStrategy strategy)
{
    private IDiscountStrategy _strategy = strategy;

    public void SetStrategy(IDiscountStrategy strategy) => _strategy = strategy;

    public decimal Calculate(decimal unitPrice, int quantity)
        => _strategy.Apply(unitPrice, quantity) * quantity;
}
