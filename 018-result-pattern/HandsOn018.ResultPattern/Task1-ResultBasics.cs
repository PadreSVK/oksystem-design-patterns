using FluentResults;

namespace HandsOn018.ResultPattern;

// =============================================================================
// Task 1 — Replace Exceptions with Result<T>
// =============================================================================
//
// The CreateOrderHandler below throws a ValidationException when the order has
// no items. The OrderController catches it with try/catch.
//
// YOUR TASKS:
//   1. Change CreateOrderHandler.Handle to return Result<Guid> instead of Guid.
//      - On validation failure → return Result.Fail("Order must contain at least one item.")
//      - On success           → return Result.Ok(newOrderId)
//   2. Update OrderController.CreateOrder to check result.IsFailed
//      instead of using try/catch.
//
// HINT: Look at the FluentResults docs for Result.Ok / Result.Fail.
// =============================================================================

public class CreateOrderHandler
{
    /// <summary>
    /// TODO: Change return type to Result&lt;Guid&gt; and replace the throw
    ///       with Result.Fail / Result.Ok.
    /// </summary>
    public Guid Handle(CreateOrderCommand command)
    {
        if (command.Items is null || command.Items.Count == 0)
            throw new ValidationException("Order must contain at least one item.");

        if (string.IsNullOrWhiteSpace(command.CustomerId))
            throw new ValidationException("CustomerId is required.");

        // Simulate order creation
        var orderId = Guid.NewGuid();
        return orderId;
    }
}

public class OrderController(CreateOrderHandler handler)
{
    /// <summary>
    /// TODO: Remove try/catch. Use result.IsFailed to decide the response.
    /// Return BadRequest with the first error message on failure, Ok with the value on success.
    /// </summary>
    public ApiResponse CreateOrder(CreateOrderCommand command)
    {
        try
        {
            var orderId = handler.Handle(command);
            return ApiResponse.Ok(orderId);
        }
        catch (ValidationException ex)
        {
            return ApiResponse.BadRequest(ex.Message);
        }
    }
}
