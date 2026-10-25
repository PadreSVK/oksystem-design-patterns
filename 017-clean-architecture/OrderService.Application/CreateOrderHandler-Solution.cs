using OrderService.Domain;

namespace OrderService.Application;

// =============================================================================
// Task 2 Solution — Application Service (Handler)
// =============================================================================

public class CreateOrderHandler(IOrderRepository repository)
{
    public async Task<CreateOrderResponse> HandleAsync(
        CreateOrderCommand command, CancellationToken ct = default)
    {
        var lines = command.Lines
            .Select(l => OrderLine.Create(l.ProductId, l.Quantity, l.UnitPrice))
            .ToList();

        var order = Order.Create(command.CustomerId, lines);

        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);

        return new CreateOrderResponse(
            order.Id,
            order.CustomerId,
            order.Lines.Count,
            order.Total);
    }
}
