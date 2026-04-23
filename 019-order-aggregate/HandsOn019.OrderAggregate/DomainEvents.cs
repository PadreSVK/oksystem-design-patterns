namespace HandsOn019.OrderAggregate;

/// <summary>
/// Domain event raised when an order is successfully placed.
/// </summary>
public sealed record OrderPlacedEvent(
    Guid OrderId,
    string CustomerId,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public OrderPlacedEvent(Guid orderId, string customerId)
        : this(orderId, customerId, DateTimeOffset.UtcNow) { }
}

/// <summary>
/// Domain event raised when an order is cancelled.
/// </summary>
public sealed record OrderCancelledEvent(
    Guid OrderId,
    string Reason,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public OrderCancelledEvent(Guid orderId, string reason)
        : this(orderId, reason, DateTimeOffset.UtcNow) { }
}
