using Mediator;

namespace HandsOn021.Application;

// =============================================================================
// Shared types (always compiled)
// =============================================================================

// --- Result monad ---

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(T value) { IsSuccess = true; Value = value; }
    private Result(string error) { IsSuccess = false; Error = error; }

    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Fail(string error) => new(error);
}

// --- Command (write side) ---

public record OrderItemDto(string ProductName, int Quantity, decimal UnitPrice);

public record PlaceOrderCommand(
    string CustomerId,
    IReadOnlyList<OrderItemDto> Items) : IRequest<Result<Guid>>;

// --- Notification (domain event published via Mediator) ---

public record OrderPlacedNotification(
    Guid OrderId,
    string CustomerId,
    IReadOnlyList<OrderItemDto> Items,
    decimal TotalAmount,
    DateTimeOffset PlacedAt) : INotification;

// --- Query (read side) ---

public record GetOrderSummaryQuery(Guid OrderId)
    : IRequest<Result<OrderSummaryDto>>;

public record OrderSummaryDto(
    Guid OrderId,
    string Status,
    decimal TotalAmount,
    int ItemCount,
    DateTimeOffset PlacedAt);
