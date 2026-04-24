namespace OrderService.Application;

// =============================================================================
// Shared DTOs — Command & Response (always compiled)
// =============================================================================

public record CreateOrderCommand(string CustomerId, List<CreateOrderLineDto> Lines);
public record CreateOrderLineDto(string ProductId, int Quantity, decimal UnitPrice);
public record CreateOrderResponse(Guid OrderId, string CustomerId, int LineCount, decimal Total);
