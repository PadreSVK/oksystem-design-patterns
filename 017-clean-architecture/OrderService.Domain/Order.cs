namespace OrderService.Domain;

// =============================================================================
// Domain Entity — Order aggregate root (always compiled)
// =============================================================================

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; } = "";
    public DateTime CreatedAt { get; private set; }
    public List<OrderLine> Lines { get; private set; } = [];

    private Order() { } // EF Core

    public static Order Create(string customerId, List<OrderLine> lines)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("CustomerId is required.", nameof(customerId));

        if (lines is null || lines.Count == 0)
            throw new ArgumentException("Order must have at least one line.", nameof(lines));

        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            Lines = lines
        };
    }

    public decimal Total => Lines.Sum(l => l.UnitPrice * l.Quantity);
}

public class OrderLine
{
    public Guid Id { get; private set; }
    public string ProductId { get; private set; } = "";
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderLine() { } // EF Core

    public static OrderLine Create(string productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        return new OrderLine
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}
