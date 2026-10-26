# Hands-On 019 — Order Aggregate & Domain Event

Build a rich domain model using DDD tactical patterns: Value Objects, Aggregate Root, and Domain Events. The domain project has **zero NuGet dependencies** — pure C#.

## Tasks

### Task 1 — Money Value Object (`Money.cs`)

1. Implement `readonly record struct Money(decimal Amount, string Currency)`.
2. Add `operator+` with a currency guard (`DomainException` if currencies differ).
3. Add `static readonly Money Zero = new(0, "USD")`.

### Task 2 — OrderItem Value Object (`OrderItem.cs`)

1. Implement `readonly record struct OrderItem(Guid ProductId, string Name, int Quantity, Money UnitPrice)`.
2. Add `LineTotal` computed property: `UnitPrice with { Amount = UnitPrice.Amount * Quantity }`.
3. Add `static Create(...)` factory that validates quantity > 0 and name not blank.

### Task 3 — Order Aggregate Root (`Order.cs`)

1. Extend `AggregateRoot<Guid>`.
2. `static Order Place(string customerId, IEnumerable<OrderItem> items)`:
   - Validates non-empty items and non-blank customerId.
   - Sets status to `Placed`, calculates `Total`.
   - Raises `OrderPlacedEvent`.
3. `void Cancel(string reason)`:
   - Throws `DomainException` if status is `Shipped`.
   - Sets status to `Cancelled`, raises `OrderCancelledEvent`.

## Running Tests

```bash
cd 019-order-aggregate
dotnet test
```

Tests will fail until all three tasks are completed. Compare with `*-Solution.cs` files.
