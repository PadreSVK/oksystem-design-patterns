using FluentResults;

namespace HandsOn018.ResultPattern;

// =============================================================================
// Task 1 Solution — Replace Exceptions with Result<T>
// =============================================================================

public class CreateOrderHandler
{
    public Result<Guid> Handle(CreateOrderCommand command)
    {
        if (command.Items is null || command.Items.Count == 0)
            return Result.Fail("Order must contain at least one item.");

        if (string.IsNullOrWhiteSpace(command.CustomerId))
            return Result.Fail("CustomerId is required.");

        var orderId = Guid.NewGuid();
        return Result.Ok(orderId);
    }
}

public class OrderController(CreateOrderHandler handler)
{
    public ApiResponse CreateOrder(CreateOrderCommand command)
    {
        var result = handler.Handle(command);

        return result.IsFailed
            ? ApiResponse.BadRequest(result.Errors.First().Message)
            : ApiResponse.Ok(result.Value);
    }
}
