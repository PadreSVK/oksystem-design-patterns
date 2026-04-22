// =============================================================================
// Hands-On 011 — Task 1 Tests: Validation Pipeline (Chain of Responsibility)
//
// These tests are always compiled and run.
// Fix Task1-ChainOfResponsibility.cs until all pass.
// =============================================================================

using Xunit;
using HandsOn.ChainMediatorIterator;

namespace HandsOn011.Tests;

public class ChainOfResponsibilityTests
{
    private static OrderRequest ValidOrder() =>
        new(Guid.NewGuid(), 100m, ["item-1", "item-2"]);

    private static IOrderValidator BuildFullChain(decimal maxAmount = 500m)
    {
        var v1 = new CustomerIdValidator();
        var v2 = new ItemsNotEmptyValidator();
        var v3 = new MaxAmountValidator(maxAmount);
        v1.SetNext(v2).SetNext(v3);
        return v1;
    }

    // ── Success paths ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ValidOrder_PassesFullChain_ReturnsSuccess()
    {
        var chain = BuildFullChain();

        var result = await chain.ValidateAsync(ValidOrder());

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task OrderExactlyAtMaxAmount_Passes()
    {
        var chain = BuildFullChain(maxAmount: 100m);
        var order = new OrderRequest(Guid.NewGuid(), 100m, ["item"]);

        var result = await chain.ValidateAsync(order);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task SingleValidator_NoNext_ReturnsSuccessWhenValid()
    {
        var validator = new CustomerIdValidator(); // no SetNext called

        var result = await validator.ValidateAsync(ValidOrder());

        Assert.True(result.IsValid);
    }

    // ── Failure + short-circuit paths ─────────────────────────────────────────

    [Fact]
    public async Task EmptyCustomerId_FailsAtCustomerIdValidator()
    {
        var chain = BuildFullChain();
        var order = new OrderRequest(Guid.Empty, 100m, ["item"]);

        var result = await chain.ValidateAsync(order);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors); // short-circuit: only one error
    }

    [Fact]
    public async Task EmptyCustomerId_ShortCircuits_DoesNotCheckItems()
    {
        var chain = BuildFullChain();
        // Both CustomerId AND Items are invalid — only one error expected (short-circuit)
        var order = new OrderRequest(Guid.Empty, 100m, []);

        var result = await chain.ValidateAsync(order);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Customer ID", result.Errors[0]);
    }

    [Fact]
    public async Task EmptyItems_FailsAtItemsNotEmptyValidator()
    {
        var chain = BuildFullChain();
        var order = new OrderRequest(Guid.NewGuid(), 100m, []);

        var result = await chain.ValidateAsync(order);

        Assert.False(result.IsValid);
        Assert.Contains("item", result.Errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AmountExceedsMax_FailsAtMaxAmountValidator()
    {
        var chain = BuildFullChain(maxAmount: 200m);
        var order = new OrderRequest(Guid.NewGuid(), 201m, ["item"]);

        var result = await chain.ValidateAsync(order);

        Assert.False(result.IsValid);
        Assert.Contains("201", result.Errors[0]);
    }

    [Fact]
    public async Task StandaloneCustomerIdValidator_EmptyGuid_ReturnsFailure()
    {
        var validator = new CustomerIdValidator();
        var order = new OrderRequest(Guid.Empty, 50m, ["x"]);

        var result = await validator.ValidateAsync(order);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task SetNext_ReturnsNext_EnablesFluentChaining()
    {
        // SetNext returns next so v1.SetNext(v2).SetNext(v3) is valid
        var v1 = new CustomerIdValidator();
        var v2 = new ItemsNotEmptyValidator();

        var returned = v1.SetNext(v2);

        Assert.Same(v2, returned); // SetNext must return the next validator
    }
}
