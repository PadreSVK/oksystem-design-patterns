namespace HandsOn019.OrderAggregate;

// =============================================================================
// Task 2 Solution — OrderItem Value Object
// =============================================================================

public readonly record struct OrderItem(Guid ProductId, string Name, int Quantity, Money UnitPrice)
{
    public Money LineTotal => UnitPrice with { Amount = UnitPrice.Amount * Quantity };

    public static OrderItem Create(Guid productId, string name, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");

        return new OrderItem(productId, name, quantity, unitPrice);
    }
}
