// =============================================================================
// Hands-On 011 — Task 1: Multi-Step Validation Pipeline (Chain of Responsibility)  (starter)
//
// Scenario: incoming order requests must pass through multiple independent
//           validation steps before processing.  Each step either short-circuits
//           with an error or passes the request down to the next handler.
//
// Your tasks — implement the three concrete validators:
//
//   CustomerIdValidator   : CustomerId must not be Guid.Empty
//   ItemsNotEmptyValidator: Items list must contain at least one item
//   MaxAmountValidator    : TotalAmount must not exceed the configured maximum
//
// Each validator must:
//   1. Check its own condition.
//   2. Return ValidationResult.Failure(...) immediately if the check fails.
//   3. Return await PassToNextAsync(request) to continue the chain.
//
// Chain of Responsibility reminder:
//   • IOrderValidator         = Handler interface
//   • OrderValidatorBase      = Abstract Handler (provided) — handles SetNext / PassToNext
//   • CustomerIdValidator etc.= Concrete Handlers (your task)
//   • The caller builds: v1.SetNext(v2).SetNext(v3)  ← fluent chaining
// =============================================================================

namespace HandsOn.ChainMediatorIterator;

// ─── Domain ───────────────────────────────────────────────────────────────────

public sealed record OrderRequest(Guid CustomerId, decimal TotalAmount, IReadOnlyList<string> Items);

public sealed record ValidationResult(bool IsValid, IReadOnlyList<string> Errors)
{
    public static ValidationResult Success { get; } = new(true, []);
    public static ValidationResult Failure(string error) => new(false, [error]);
}

// ─── Handler interface ────────────────────────────────────────────────────────

public interface IOrderValidator
{
    /// <summary>Sets the next handler in the chain; returns next for fluent chaining.</summary>
    IOrderValidator SetNext(IOrderValidator next);

    Task<ValidationResult> ValidateAsync(OrderRequest request);
}

// ─── Abstract base handler (provided — do not modify) ────────────────────────

public abstract class OrderValidatorBase : IOrderValidator
{
    private IOrderValidator? _next;

    public IOrderValidator SetNext(IOrderValidator next)
    {
        _next = next;
        return next; // enables: v1.SetNext(v2).SetNext(v3)
    }

    /// <summary>
    /// Passes the request to the next handler in the chain.
    /// Returns Success if there is no next handler (end of chain).
    /// </summary>
    protected Task<ValidationResult> PassToNextAsync(OrderRequest request)
        => _next?.ValidateAsync(request) ?? Task.FromResult(ValidationResult.Success);

    public abstract Task<ValidationResult> ValidateAsync(OrderRequest request);
}

// ─── Concrete validators (your task) ─────────────────────────────────────────

public sealed class CustomerIdValidator : OrderValidatorBase
{
    public override Task<ValidationResult> ValidateAsync(OrderRequest request)
    {
        // TODO: if (request.CustomerId == Guid.Empty)
        //           return Task.FromResult(ValidationResult.Failure("Customer ID is required."))
        // TODO: return PassToNextAsync(request)
        throw new NotImplementedException();
    }
}

public sealed class ItemsNotEmptyValidator : OrderValidatorBase
{
    public override Task<ValidationResult> ValidateAsync(OrderRequest request)
    {
        // TODO: if (request.Items.Count == 0)
        //           return Task.FromResult(ValidationResult.Failure("Order must contain at least one item."))
        // TODO: return PassToNextAsync(request)
        throw new NotImplementedException();
    }
}

public sealed class MaxAmountValidator(decimal maxAmount) : OrderValidatorBase
{
    private readonly decimal _maxAmount = maxAmount;

    public override Task<ValidationResult> ValidateAsync(OrderRequest request)
    {
        // TODO: if (request.TotalAmount > _maxAmount)
        //           return Task.FromResult(ValidationResult.Failure(
        //               $"Order total {request.TotalAmount} exceeds the maximum of {_maxAmount}."))
        // TODO: return PassToNextAsync(request)
        throw new NotImplementedException();
    }
}
