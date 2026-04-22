using FluentResults;
using MediatR;

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

// --- MediatR command & handler (used by Task 3) ---

public record PlaceOrderMediatRCommand(string CustomerId, List<OrderItemDto> Items)
    : IRequest<Result<Guid>>;

public class PlaceOrderMediatRHandler : IRequestHandler<PlaceOrderMediatRCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(PlaceOrderMediatRCommand request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0)
            return Task.FromResult(Result.Fail<Guid>("Order must contain at least one item."));

        return Task.FromResult(Result.Ok(Guid.NewGuid()));
    }
}
