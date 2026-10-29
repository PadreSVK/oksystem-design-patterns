using FluentResults;
using Mediator;

namespace HandsOn018.ResultPattern;

// =============================================================================
// Shared types used across all tasks — always compiled.
// =============================================================================

public record CreateOrderCommand(string CustomerId, List<OrderItemDto> Items);
public record OrderItemDto(string ProductId, int Quantity, decimal UnitPrice);
public record PlaceOrderContext(string CustomerId, List<OrderItemDto> Items, Guid? OrderId = null);

public class ValidationException(string message) : Exception(message);

public record ApiResponse(int StatusCode, object? Value, string? Error)
{
    public static ApiResponse Ok(object value) => new(200, value, null);
    public static ApiResponse BadRequest(string error) => new(400, null, error);
}

// --- Mediator command & handler (used by Task 3) ---

public record PlaceOrderMediatorCommand(string CustomerId, List<OrderItemDto> Items)
    : IRequest<Result<Guid>>;

public class PlaceOrderMediatorHandler : IRequestHandler<PlaceOrderMediatorCommand, Result<Guid>>
{
    public ValueTask<Result<Guid>> Handle(PlaceOrderMediatorCommand request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0)
            return ValueTask.FromResult(Result.Fail<Guid>("Order must contain at least one item."));

        return ValueTask.FromResult(Result.Ok(Guid.NewGuid()));
    }
}
