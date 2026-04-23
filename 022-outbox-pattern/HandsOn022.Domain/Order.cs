namespace HandsOn022.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; } = "";
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];

    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);

    private Order() { }

    public static Order Place(string customerId, List<OrderItem> items)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("CustomerId is required.", nameof(customerId));

        if (items is null || items.Count == 0)
            throw new ArgumentException("Order must have at least one item.", nameof(items));

        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Placed,
            Items = items
        };
    }
}
