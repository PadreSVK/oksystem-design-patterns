using HandsOn020.Domain;

namespace HandsOn020.Application;

// =============================================================================
// Task 2 — Implement PlaceOrderHandler (Application Service)
// =============================================================================
//
// The handler orchestrates the use case: map DTOs → domain objects, call
// the repository, and return the result. It depends ONLY on Domain interfaces
// (IOrderRepository), never on Infrastructure (EF Core, DbContext).
//
// YOUR TASKS:
//   1. Accept IOrderRepository via primary constructor.
//   2. In HandleAsync:
//      a. Map each OrderItemDto → OrderItem.Create(...)
//      b. Create the Order aggregate via Order.Place(...)
//      c. Add to repository and save
//      d. Return a PlaceOrderResult with order details
//
// KEY INSIGHT: The handler uses IOrderRepository (Domain interface).
//              It has NO reference to EF Core, DbContext, or Include.
//              Infrastructure details are encapsulated in the repository.
//
// ANTI-PATTERN (IQueryable leak):
//   ❌ var orders = _repo.Query()
//          .Include(o => o.Items)    // EF Core-specific in Application layer!
//          .Where(o => o.Status == OrderStatus.Placed)
//          .ToListAsync();
//
//   ✅ var orders = await _repo.GetPlacedOrdersAsync(ct);
// =============================================================================

public class PlaceOrderHandler
{
    // TODO: Accept IOrderRepository via primary constructor

    public Task<PlaceOrderResult> HandleAsync(
        PlaceOrderCommand command, CancellationToken ct = default)
    {
        // TODO: Map command.Items → List<OrderItem> using OrderItem.Create
        // TODO: Create order via Order.Place(command.CustomerId, items)
        // TODO: Add to repository + save
        // TODO: Return PlaceOrderResult(order.Id, order.CustomerId, order.Items.Count, order.Total)

        throw new NotImplementedException();
    }
}
