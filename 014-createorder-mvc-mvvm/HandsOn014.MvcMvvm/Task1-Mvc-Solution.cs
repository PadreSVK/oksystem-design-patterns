using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HandsOn.MvcMvvm;

// ──── Request / Response DTOs ────────────────────────────────────────

public record CreateOrderLineRequest(Guid ProductId, int Quantity);

public class CreateOrderRequest
{
    [Required] public string CustomerId { get; set; } = "";
    [Required, MinLength(1)] public List<CreateOrderLineRequest> Lines { get; set; } = [];
}

public record CreateOrderLineDto(Guid ProductId, int Quantity);

public record CreateOrderCommand(
    string CustomerId,
    IReadOnlyList<CreateOrderLineDto> Lines);

public record CreateOrderResponse(Guid OrderId, string CustomerId, int LineCount);

// ──── Service result wrapper ─────────────────────────────────────────

public class ServiceResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }

    public static ServiceResult<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static ServiceResult<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}

// ──── Service contract ───────────────────────────────────────────────

public interface IOrderService
{
    Task<ServiceResult<CreateOrderResponse>> CreateAsync(
        CreateOrderCommand command, CancellationToken ct = default);
}

// ──── Controller ─────────────────────────────────────────────────────

[ApiController]
[Route("api/orders")]
public class CreateOrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public CreateOrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id) => Ok();

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new CreateOrderCommand(
            request.CustomerId,
            request.Lines
                .Select(l => new CreateOrderLineDto(l.ProductId, l.Quantity))
                .ToList());

        var result = await _orderService.CreateAsync(command, ct);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(result.Error ?? "Unable to create order.");

        return CreatedAtAction(nameof(GetById), new { id = result.Value.OrderId }, result.Value);
    }
}
