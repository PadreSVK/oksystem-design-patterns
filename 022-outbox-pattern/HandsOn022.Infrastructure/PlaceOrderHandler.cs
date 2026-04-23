using System.Text.Json;
using HandsOn022.Domain;

namespace HandsOn022.Infrastructure;

// =============================================================================
// Task 1 — PlaceOrderHandler (Outbox Pattern — Atomic Write)
// =============================================================================
//
// The Outbox Pattern ensures that domain state changes AND the corresponding
// event message are written in the SAME database transaction. This guarantees
// at-least-once delivery without distributed transactions.
//
// YOUR TASKS:
//   1. Accept AppDbContext via primary constructor.
//   2. In HandleAsync:
//      a. Map command.Items → List<OrderItem> using OrderItem.Create(...)
//      b. Create the Order aggregate via Order.Place(...)
//      c. Add the order to db.Orders
//      d. Create an OutboxMessage with:
//         - Type = "OrderPlaced"
//         - Payload = JSON-serialized object with OrderId, CustomerId, Total
//         - Sent = false (default)
//      e. Add the OutboxMessage to db.OutboxMessages
//      f. Call db.SaveChangesAsync(ct) — ONE call saves BOTH atomically
//      g. Return PlaceOrderResult
//
// KEY INSIGHT: Both the Order and the OutboxMessage are saved in a single
//              SaveChangesAsync call. If either fails, both are rolled back.
//              The background relay (Task 2) will pick up the outbox message
//              and publish it to the broker.
//
// ANTI-PATTERN:
//   ❌ Save order first, THEN publish to broker — if publish fails after
//      save, the message is lost forever.
//   ✅ Save order + outbox message atomically, relay publishes later.
// =============================================================================

public class PlaceOrderHandler
{
    // TODO: Accept AppDbContext via primary constructor

    public Task<PlaceOrderResult> HandleAsync(
        PlaceOrderCommand command, CancellationToken ct = default)
    {
        // TODO: Map command.Items → OrderItem.Create(...)
        // TODO: Order.Place(command.CustomerId, items)
        // TODO: db.Orders.Add(order)
        // TODO: db.OutboxMessages.Add(new OutboxMessage
        //       {
        //           Type = "OrderPlaced",
        //           Payload = JsonSerializer.Serialize(new { order.Id, order.CustomerId, order.Total })
        //       })
        // TODO: await db.SaveChangesAsync(ct)  ← single atomic commit
        // TODO: return PlaceOrderResult(...)

        throw new NotImplementedException();
    }
}
