namespace HandsOn019.OrderAggregate;

// =============================================================================
// Task 1 — Money Value Object
// =============================================================================
//
// Money is a VALUE OBJECT — equality is by value, not by reference.
// Using `readonly record struct` gives us immutability and value equality for free.
//
// YOUR TASKS:
//   1. Implement Money as a readonly record struct with Amount (decimal) and
//      Currency (string) properties.
//   2. Implement operator+ that adds two Money values:
//      - If currencies differ → throw DomainException("Cannot add different currencies.")
//      - Otherwise → return new Money with summed amounts, same currency.
//   3. Add a static readonly Zero field: new Money(0, "USD").
//
// HINT: `readonly record struct` gives you Equals, GetHashCode, ToString for free.
// =============================================================================

// TODO: Implement readonly record struct Money(decimal Amount, string Currency)

// TODO: Add operator+(Money a, Money b) with currency guard

// TODO: Add static readonly Money Zero = new(0, "USD");
