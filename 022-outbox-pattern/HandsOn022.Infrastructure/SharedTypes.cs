namespace HandsOn022.Infrastructure;

// =============================================================================
// Shared DTOs (always compiled)
// =============================================================================

public record PlaceOrderCommand(string CustomerId, List<OrderItemDto> Items);
public record OrderItemDto(string ProductName, int Quantity, decimal UnitPrice);
public record PlaceOrderResult(Guid OrderId, string CustomerId, int ItemCount, decimal Total);
