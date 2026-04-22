using FluentResults;

namespace HandsOn018.ResultPattern;

// =============================================================================
// Task 2 — Chain Fallible Operations with Bind
// =============================================================================
//
// The PlaceOrderWorkflow below calls three steps sequentially. Each step can
// fail, but right now failures are signaled via exceptions.
//
// YOUR TASKS:
//   1. Make each method return Result<PlaceOrderContext> instead of throwing.
//   2. Chain them using .Bind() so the pipeline short-circuits on first failure:
//
//        var result = ValidateCommand(cmd)
//            .Bind(ctx => CheckInventory(ctx))
//            .Bind(ctx => CreateOrder(ctx));
//
//   3. The Execute method should return Result<Guid> with the created OrderId.
//
// HINT: Result<T>.Bind(Func<T, Result<U>>) transforms the value if Ok,
//       or propagates the failure if already failed.
// =============================================================================

public class PlaceOrderWorkflow
{
    /// <summary>
    /// TODO: Rewrite to chain ValidateCommand → CheckInventory → CreateOrder
    ///       using .Bind() and return Result&lt;Guid&gt;.
    /// </summary>
    public Guid Execute(CreateOrderCommand command)
    {
        var context = ValidateCommand(command);
        context = CheckInventory(context);
        context = CreateOrder(context);
        return context.OrderId!.Value;
    }

    private PlaceOrderContext ValidateCommand(CreateOrderCommand command)
    {
        if (command.Items is null || command.Items.Count == 0)
            throw new ValidationException("Order must contain at least one item.");

        if (command.Items.Any(i => i.Quantity <= 0))
            throw new ValidationException("All items must have a positive quantity.");

        return new PlaceOrderContext(command.CustomerId, command.Items);
    }

    private PlaceOrderContext CheckInventory(PlaceOrderContext context)
    {
        // Simulate: item "OUT-OF-STOCK" always fails
        if (context.Items.Any(i => i.ProductId == "OUT-OF-STOCK"))
            throw new InvalidOperationException("Insufficient inventory for one or more items.");

        return context;
    }

    private PlaceOrderContext CreateOrder(PlaceOrderContext context)
    {
        var orderId = Guid.NewGuid();
        return context with { OrderId = orderId };
    }
}
