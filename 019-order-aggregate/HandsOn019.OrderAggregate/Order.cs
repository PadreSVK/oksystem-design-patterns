namespace HandsOn019.OrderAggregate;

// =============================================================================
// Task 3 — Order Aggregate Root
// =============================================================================
//
// Order is the AGGREGATE ROOT — it enforces all invariants and raises domain
// events. External code can only modify the aggregate through its public methods.
//
// YOUR TASKS:
//   1. Make Order extend AggregateRoot<Guid>.
//   2. Implement a static Place factory method:
//      - Accepts customerId (string) and items (IEnumerable<OrderItem>)
//      - If items is empty → throw DomainException("Order must have at least one item.")
//      - If customerId is blank → throw DomainException("CustomerId is required.")
//      - Sets Status = OrderStatus.Placed
//      - Calculates Total by summing all LineTotal values
//      - Raises OrderPlacedEvent(order.Id, customerId)
//      - Returns the new Order
//   3. Implement Cancel(string reason):
//      - If Status == OrderStatus.Shipped → throw DomainException("Cannot cancel a shipped order.")
//      - Sets Status = OrderStatus.Cancelled
//      - Raises OrderCancelledEvent(Id, reason)
//   4. Add a read-only Items collection (IReadOnlyList<OrderItem>).
//
// HINT: Use a private constructor and a static factory for creation invariants.
//       Use RaiseDomainEvent() from the AggregateRoot base class.
// =============================================================================

public class Order
    // TODO: extend AggregateRoot<Guid>
{
    // TODO: private List<OrderItem> _items + public IReadOnlyList<OrderItem> Items

    // TODO: public OrderStatus Status { get; private set; }
    // TODO: public string CustomerId { get; private set; }
    // TODO: public Money Total { get; private set; }
    // TODO: public DateTime CreatedAt { get; private set; }

    // TODO: private Order() constructor

    // TODO: public static Order Place(string customerId, IEnumerable<OrderItem> items)

    // TODO: public void Cancel(string reason)
}
