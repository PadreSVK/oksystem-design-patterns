# Hands-On 018 — Replace Exceptions with Result\<T\>

This lab teaches you to replace exception-based error handling with the **Result pattern** using the [FluentResults](https://github.com/altmann/FluentResults) library. You will refactor command handlers, chain fallible operations with `Bind`, and build a [Mediator](https://github.com/martinothamar/Mediator) pipeline behavior that logs failures — all without a single `try/catch`.

## Prerequisites

- .NET 8 SDK
- Familiarity with Mediator basics (from Hands-On 015)

## Tasks

### Task 1 — Result Basics (`Task1-ResultBasics.cs`)

| Before | After |
|--------|-------|
| `CreateOrderHandler` throws `ValidationException` | Returns `Result<Guid>` with `Result.Fail` / `Result.Ok` |
| `OrderController` uses `try/catch` | Checks `result.IsFailed` |

1. Change `Handle` return type from `Guid` to `Result<Guid>`.
2. Replace `throw` statements with `Result.Fail("...")`.
3. Return `Result.Ok(orderId)` on success.
4. Update `OrderController.CreateOrder` to use `result.IsFailed` instead of `try/catch`.

### Task 2 — Bind Chaining (`Task2-BindChaining.cs`)

Chain three fallible steps using `.Bind()` (railway-oriented programming):

```
ValidateCommand → CheckInventory → CreateOrder
```

1. Convert each private method to return `Result<PlaceOrderContext>`.
2. Chain them: `ValidateCommand(cmd).Bind(ctx => CheckInventory(ctx)).Bind(ctx => CreateOrder(ctx))`.
3. `Execute` should return `Result<Guid>` — the pipeline short-circuits on first failure.

### Task 3 — Mediator Pipeline Behavior (`Task3-PipelineBehavior.cs`)

Implement `ResultLoggingBehavior<TRequest, TResponse> : IPipelineBehavior` that:

1. Constrains `TRequest : notnull, IMessage` and `TResponse : ResultBase`.
2. Calls `next(request, cancellationToken)` to get the response.
3. If `response.IsFailed`, logs a warning: `"Request {RequestName} failed: {Errors}"`.

## Running Tests

```bash
cd hands-on/018-result-pattern
dotnet test
```

Tests will **fail to compile** until you refactor the exercise files. Compare your work with the `*-Solution.cs` files.
