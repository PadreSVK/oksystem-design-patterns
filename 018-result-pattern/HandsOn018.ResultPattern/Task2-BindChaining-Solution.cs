using FluentResults;

namespace HandsOn018.ResultPattern;

// =============================================================================
// Task 2 Solution — Chain Fallible Operations with Bind
// =============================================================================

public class PlaceOrderWorkflow
{
    public Result<Guid> Execute(CreateOrderCommand command)
    {
        var result = ValidateCommand(command)
            .Bind(ctx => CheckInventory(ctx))
            .Bind(ctx => CreateOrder(ctx));

        return result.IsSuccess
            ? Result.Ok(result.Value.OrderId!.Value)
            : Result.Fail<Guid>(result.Errors);
    }

    private Result<PlaceOrderContext> ValidateCommand(CreateOrderCommand command)
    {
        if (command.Items is null || command.Items.Count == 0)
            return Result.Fail("Order must contain at least one item.");

        if (command.Items.Any(i => i.Quantity <= 0))
            return Result.Fail("All items must have a positive quantity.");

        return Result.Ok(new PlaceOrderContext(command.CustomerId, command.Items));
    }

    private Result<PlaceOrderContext> CheckInventory(PlaceOrderContext context)
    {
        if (context.Items.Any(i => i.ProductId == "OUT-OF-STOCK"))
            return Result.Fail("Insufficient inventory for one or more items.");

        return Result.Ok(context);
    }

    private Result<PlaceOrderContext> CreateOrder(PlaceOrderContext context)
    {
        var orderId = Guid.NewGuid();
        return Result.Ok(context with { OrderId = orderId });
    }
}
