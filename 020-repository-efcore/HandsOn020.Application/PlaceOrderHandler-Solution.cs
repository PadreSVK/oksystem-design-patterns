using HandsOn020.Domain;

namespace HandsOn020.Application;

// =============================================================================
// Task 2 Solution — PlaceOrderHandler
// =============================================================================

public class PlaceOrderHandler(IOrderRepository repository)
{
    public async Task<PlaceOrderResult> HandleAsync(
        PlaceOrderCommand command, CancellationToken ct = default)
    {
        var items = command.Items
            .Select(i => OrderItem.Create(i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        var order = Order.Place(command.CustomerId, items);

        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);

        return new PlaceOrderResult(
            order.Id,
            order.CustomerId,
            order.Items.Count,
            order.Total);
    }
}
