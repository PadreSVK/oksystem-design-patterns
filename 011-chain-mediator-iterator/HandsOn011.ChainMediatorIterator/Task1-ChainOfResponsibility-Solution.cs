// =============================================================================
// Hands-On 011 — Task 1: Multi-Step Validation Pipeline (Chain of Responsibility)  (solution)
// =============================================================================

namespace HandsOn.ChainMediatorIterator;

public sealed record OrderRequest(Guid CustomerId, decimal TotalAmount, IReadOnlyList<string> Items);

public sealed record ValidationResult(bool IsValid, IReadOnlyList<string> Errors)
{
    public static ValidationResult Success { get; } = new(true, []);
    public static ValidationResult Failure(string error) => new(false, [error]);
}

public interface IOrderValidator
{
    IOrderValidator SetNext(IOrderValidator next);
    Task<ValidationResult> ValidateAsync(OrderRequest request);
}

public abstract class OrderValidatorBase : IOrderValidator
{
    private IOrderValidator? _next;

    public IOrderValidator SetNext(IOrderValidator next)
    {
        _next = next;
        return next;
    }

    protected Task<ValidationResult> PassToNextAsync(OrderRequest request)
        => _next?.ValidateAsync(request) ?? Task.FromResult(ValidationResult.Success);

    public abstract Task<ValidationResult> ValidateAsync(OrderRequest request);
}

// ✅ Concrete handlers — each checks one condition, then passes on or short-circuits

public sealed class CustomerIdValidator : OrderValidatorBase
{
    public override Task<ValidationResult> ValidateAsync(OrderRequest request)
    {
        if (request.CustomerId == Guid.Empty)
            return Task.FromResult(ValidationResult.Failure("Customer ID is required."));

        return PassToNextAsync(request);
    }
}

public sealed class ItemsNotEmptyValidator : OrderValidatorBase
{
    public override Task<ValidationResult> ValidateAsync(OrderRequest request)
    {
        if (request.Items.Count == 0)
            return Task.FromResult(ValidationResult.Failure("Order must contain at least one item."));

        return PassToNextAsync(request);
    }
}

public sealed class MaxAmountValidator(decimal maxAmount) : OrderValidatorBase
{
    public override Task<ValidationResult> ValidateAsync(OrderRequest request)
    {
        if (request.TotalAmount > maxAmount)
            return Task.FromResult(ValidationResult.Failure(
                $"Order total {request.TotalAmount} exceeds the maximum of {maxAmount}."));

        return PassToNextAsync(request);
    }
}
