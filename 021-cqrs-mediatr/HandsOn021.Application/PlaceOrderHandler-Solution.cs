using HandsOn021.Domain;
using Mediator;

namespace HandsOn021.Application;

// =============================================================================
// Task 1 Solution — PlaceOrderHandler (Command Handler)
// =============================================================================

public class PlaceOrderHandler(IOrderRepository repository, IPublisher publisher)
    : IRequestHandler<PlaceOrderCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        PlaceOrderCommand command, CancellationToken ct)
    {
        var items = command.Items
            .Select(i => OrderItem.Create(i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        var order = Order.Place(command.CustomerId, items);

        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);

        await publisher.Publish(
            new OrderPlacedNotification(
                order.Id,
                command.CustomerId,
                command.Items,
                order.Total,
                DateTimeOffset.UtcNow),
            ct);

        return Result<Guid>.Ok(order.Id);
    }
}
