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
        // TODO Task 1 — Implement the action:
        // 1. Check ModelState.IsValid → return BadRequest(ModelState) if invalid
        // 2. Map CreateOrderRequest → CreateOrderCommand
        // 3. Call _orderService.CreateAsync(command, ct)
        // 4. If !result.IsSuccess → return BadRequest(result.Error)
        // 5. Return CreatedAtAction(nameof(GetById), new { id = ... }, result.Value)
        throw new NotImplementedException();
    }
}
