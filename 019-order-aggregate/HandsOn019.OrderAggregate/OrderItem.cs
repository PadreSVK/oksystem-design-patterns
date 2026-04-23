namespace HandsOn019.OrderAggregate;

// =============================================================================
// Task 2 — OrderItem Value Object
// =============================================================================
//
// OrderItem is a value object representing a line in an order.
// It uses Money for the unit price and enforces quantity invariants.
//
// YOUR TASKS:
//   1. Implement OrderItem as a readonly record struct with:
//      - Guid ProductId
//      - string Name
//      - int Quantity
//      - Money UnitPrice
//   2. Add a computed property LineTotal that returns UnitPrice * Quantity
//      (hint: use `UnitPrice with { Amount = UnitPrice.Amount * Quantity }`)
//   3. Add a static Create factory method that validates:
//      - Quantity <= 0 → throw DomainException("Quantity must be positive.")
//      - string.IsNullOrWhiteSpace(name) → throw DomainException("Product name is required.")
//
// HINT: record structs have a default parameterless constructor — use a
//       static factory to enforce invariants.
// =============================================================================

// TODO: Implement readonly record struct OrderItem(Guid ProductId, string Name, int Quantity, Money UnitPrice)

// TODO: Add LineTotal computed property

// TODO: Add static Create factory with validation
