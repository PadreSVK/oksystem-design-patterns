using System.Text.Json;
using HandsOn022.Domain;

namespace HandsOn022.Infrastructure;

// =============================================================================
// Task 1 Solution — PlaceOrderHandler (Outbox Pattern)
// =============================================================================

public class PlaceOrderHandler(AppDbContext db)
{
    public async Task<PlaceOrderResult> HandleAsync(
        PlaceOrderCommand command, CancellationToken ct = default)
    {
        var items = command.Items
            .Select(i => OrderItem.Create(i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        var order = Order.Place(command.CustomerId, items);

        db.Orders.Add(order);

        db.OutboxMessages.Add(new OutboxMessage
        {
            Type = "OrderPlaced",
            Payload = JsonSerializer.Serialize(new
            {
                OrderId = order.Id,
                order.CustomerId,
                order.Total
            })
        });

        await db.SaveChangesAsync(ct);

        return new PlaceOrderResult(
            order.Id,
            order.CustomerId,
            order.Items.Count,
            order.Total);
    }
}
