# Hands-On 020 — IOrderRepository with EF Core

## Objectives

- Implement `IOrderRepository` using EF Core (`Include`, `SingleOrDefaultAsync`, `Where`)
- Build an Application-layer handler that depends **only** on the Domain interface
- Understand why repositories must **never** return `IQueryable<T>`

## Structure

| Project | Layer | Purpose |
|---|---|---|
| `HandsOn020.Domain` | Domain | `Order`, `OrderItem`, `IOrderRepository` — zero NuGet |
| `HandsOn020.Infrastructure` | Infrastructure | `AppDbContext`, `EfOrderRepository` (EF Core) |
| `HandsOn020.Application` | Application | `PlaceOrderHandler` — depends only on Domain |
| `HandsOn020.Tests` | Tests | 12 xUnit tests (repo, handler, architecture) |

## Tasks

### Task 1 — `EfOrderRepository` (Infrastructure)

Open `HandsOn020.Infrastructure/EfOrderRepository.cs` and implement `IOrderRepository`:

1. Accept `AppDbContext` via primary constructor
2. `FindByIdWithItemsAsync` — use `Include(o => o.Items)` + `SingleOrDefaultAsync`
3. `AddAsync` — use `db.Orders.AddAsync`
4. `GetPlacedOrdersAsync` — filter by `OrderStatus.Placed`, include items, return `IReadOnlyList<Order>`
5. `SaveChangesAsync` — delegate to `db.SaveChangesAsync`

### Task 2 — `PlaceOrderHandler` (Application)

Open `HandsOn020.Application/PlaceOrderHandler.cs` and implement the use case:

1. Accept `IOrderRepository` via primary constructor
2. Map `OrderItemDto` → `OrderItem.Create(...)`
3. Create the aggregate via `Order.Place(...)`
4. Add to repository + save
5. Return `PlaceOrderResult`

## Key Concept — IQueryable Leak

```csharp
// ❌ Anti-pattern: IQueryable escapes the repository
public interface IOrderRepository
{
    IQueryable<Order> Query(); // caller builds any query — you lose control
}

// ✅ Correct: repository returns domain-meaningful results
public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetPlacedOrdersAsync(CancellationToken ct);
}
```

## Running Tests

```bash
dotnet test --verbosity normal
```
