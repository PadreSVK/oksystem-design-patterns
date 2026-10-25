namespace HandsOn019.OrderAggregate;

// =============================================================================
// Task 3 Solution — Order Aggregate Root
// =============================================================================

public class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public OrderStatus Status { get; private set; }
    public string CustomerId { get; private set; } = "";
    public Money Total { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { }

    public static Order Place(string customerId, IEnumerable<OrderItem> items)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new DomainException("CustomerId is required.");

        var itemList = items.ToList();

        if (itemList.Count == 0)
            throw new DomainException("Order must have at least one item.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Placed
        };

        order._items.AddRange(itemList);
        order.Total = itemList.Aggregate(Money.Zero, (sum, item) => sum + item.LineTotal);

        order.RaiseDomainEvent(new OrderPlacedEvent(order.Id, customerId));

        return order;
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Shipped)
            throw new DomainException("Cannot cancel a shipped order.");

        Status = OrderStatus.Cancelled;
        RaiseDomainEvent(new OrderCancelledEvent(Id, reason));
    }
}
