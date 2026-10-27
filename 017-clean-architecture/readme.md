# Hands-On 017 — Clean Architecture Solution

Build a four-layer Clean Architecture solution and verify that the **Dependency Rule** is enforced: outer layers depend on inner layers, never the reverse.

## Architecture

```
┌──────────────────────────────────────────────────┐
│                    API / Tests                   │
│         (references Infrastructure)              │
├──────────────────────────────────────────────────┤
│               Infrastructure                     │
│  EfOrderRepository, OrderDbContext               │
│         (references Application)                 │
├──────────────────────────────────────────────────┤
│                Application                       │
│        CreateOrderHandler, DTOs                  │
│          (references Domain)                     │
├──────────────────────────────────────────────────┤
│                  Domain                          │
│   Order, OrderLine, IOrderRepository             │
│        (ZERO external dependencies)              │
└──────────────────────────────────────────────────┘
```

## Tasks

### Task 1 — Domain: Repository Interface (`OrderService.Domain/IOrderRepository.cs`)

1. Define `IRepository<T>` with `GetByIdAsync`, `AddAsync`, `SaveChangesAsync`.
2. Define `IOrderRepository : IRepository<Order>`.
3. Verify: `Domain.csproj` must have **zero** `PackageReference` entries.

### Task 2 — Application: Handler (`OrderService.Application/CreateOrderHandler.cs`)

1. Accept `IOrderRepository` via constructor.
2. Map `CreateOrderCommand` → domain `Order` entity using `Order.Create` + `OrderLine.Create`.
3. Persist via repository and return `CreateOrderResponse`.
4. Must **not** reference `OrderService.Infrastructure`.

### Task 3 — Infrastructure: Repository (`OrderService.Infrastructure/EfOrderRepository.cs`)

1. Implement `IOrderRepository` using `OrderDbContext`.
2. `GetByIdAsync` should use `.Include(o => o.Lines)`.
3. `AddAsync` delegates to `context.Orders.AddAsync`.
4. `SaveChangesAsync` delegates to `context.SaveChangesAsync`.

## Running Tests

```bash
cd 017-clean-architecture
dotnet test
```

Tests include **architecture tests** that verify the dependency rule is correct, plus behavior tests for the handler and repository. Tests will fail until you complete all three tasks.
