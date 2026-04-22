using FluentResults;
using HandsOn018.ResultPattern;
using Xunit;

namespace HandsOn018.Tests;

public class BindChainingTests
{
    private readonly PlaceOrderWorkflow _workflow = new();

    [Fact]
    public void Execute_ValidCommand_ReturnsOkResultWithGuid()
    {
        var command = new CreateOrderCommand("CUST-1",
        [
            new OrderItemDto("PROD-A", 2, 9.99m)
        ]);

        var result = _workflow.Execute(command);

        Assert.IsType<Result<Guid>>(result);
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Fact]
    public void Execute_EmptyItems_ReturnsValidationFailure()
    {
        var command = new CreateOrderCommand("CUST-1", []);

        var result = _workflow.Execute(command);

        Assert.IsType<Result<Guid>>(result);
        Assert.True(result.IsFailed);
        Assert.Contains("at least one item", result.Errors.First().Message);
    }

    [Fact]
    public void Execute_NegativeQuantity_ReturnsValidationFailure()
    {
        var command = new CreateOrderCommand("CUST-1",
        [
            new OrderItemDto("PROD-A", -1, 5.00m)
        ]);

        var result = _workflow.Execute(command);

        Assert.True(result.IsFailed);
        Assert.Contains("positive quantity", result.Errors.First().Message);
    }

    [Fact]
    public void Execute_OutOfStockItem_ReturnsInventoryFailure()
    {
        var command = new CreateOrderCommand("CUST-1",
        [
            new OrderItemDto("OUT-OF-STOCK", 1, 5.00m)
        ]);

        var result = _workflow.Execute(command);

        Assert.True(result.IsFailed);
        Assert.Contains("inventory", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Execute_ShortCircuits_SkipsLaterStepsOnFailure()
    {
        // If validation fails, CheckInventory and CreateOrder should never run.
        // We verify by checking the error is the validation error, not inventory.
        var command = new CreateOrderCommand("CUST-1", []);

        var result = _workflow.Execute(command);

        Assert.True(result.IsFailed);
        Assert.DoesNotContain("inventory",
            result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }
}
