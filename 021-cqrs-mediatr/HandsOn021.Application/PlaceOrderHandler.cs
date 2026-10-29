using HandsOn021.Domain;
using Mediator;

namespace HandsOn021.Application;

// =============================================================================
// Task 1 — PlaceOrderHandler (Command Handler — Write Side)
// =============================================================================
//
// This is the COMMAND handler — it orchestrates domain logic and persistence.
// It follows the CQRS write-side pattern: receive command → execute domain
// logic → persist → publish notification for the read-side projector.
//
// YOUR TASKS:
//   1. Implement IRequestHandler<PlaceOrderCommand, Result<Guid>>.
//   2. Accept IOrderRepository and IPublisher via constructor.
//   3. In Handle:
//      a. Map command.Items → List<OrderItem> using OrderItem.Create(...)
//      b. Create the Order aggregate via Order.Place(...)
//      c. Add to repository and save
//      d. Publish an OrderPlacedNotification via _publisher.Publish(...)
//         with: order.Id, command.CustomerId, command.Items, order.Total,
//               DateTimeOffset.UtcNow
//      e. Return Result<Guid>.Ok(order.Id)
//
// KEY INSIGHT: The command handler uses IOrderRepository (Domain interface)
//              and IPublisher (Mediator) — no EF Core references here.
//              The published notification will be handled by the projector
//              to update the read model.
//
// HINT: See the slide "CQRS — Mediator Command Handler (PlaceOrder)"
// =============================================================================

public class PlaceOrderHandler
    // TODO: implement IRequestHandler<PlaceOrderCommand, Result<Guid>>
{
    // TODO: Accept IOrderRepository and IPublisher via constructor

    // TODO: Implement Handle method
    //   1. Map command.Items → OrderItem.Create(...)
    //   2. Order.Place(command.CustomerId, items)
    //   3. repo.AddAsync + repo.SaveChangesAsync
    //   4. publisher.Publish(new OrderPlacedNotification(...))
    //   5. return Result<Guid>.Ok(order.Id)
}
