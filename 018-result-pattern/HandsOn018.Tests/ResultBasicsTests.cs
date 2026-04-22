using FluentResults;
using HandsOn018.ResultPattern;
using Xunit;

namespace HandsOn018.Tests;

public class ResultBasicsTests
{
    private readonly CreateOrderHandler _handler = new();

    [Fact]
    public void Handle_ValidCommand_ReturnsOkWithGuid()
    {
        // Arrange
        var command = new CreateOrderCommand("CUST-1",
        [
            new OrderItemDto("PROD-A", 2, 9.99m)
        ]);

        // Act
        var result = _handler.Handle(command);

        // Assert — after refactoring, result should be Result<Guid>
        // If the student hasn't refactored yet, this will fail to compile.
        Assert.IsType<Result<Guid>>(result);
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Fact]
    public void Handle_EmptyItems_ReturnsFailure()
    {
        var command = new CreateOrderCommand("CUST-1", []);

        var result = _handler.Handle(command);

        Assert.IsType<Result<Guid>>(result);
        Assert.True(result.IsFailed);
        Assert.Contains("at least one item", result.Errors.First().Message);
    }

    [Fact]
    public void Handle_MissingCustomerId_ReturnsFailure()
    {
        var command = new CreateOrderCommand("",
        [
            new OrderItemDto("PROD-A", 1, 5.00m)
        ]);

        var result = _handler.Handle(command);

        Assert.True(result.IsFailed);
        Assert.Contains("CustomerId", result.Errors.First().Message);
    }

    [Fact]
    public void Controller_ValidOrder_Returns200()
    {
        var handler = new CreateOrderHandler();
        var controller = new OrderController(handler);

        var command = new CreateOrderCommand("CUST-1",
        [
            new OrderItemDto("PROD-A", 1, 10.00m)
        ]);

        var response = controller.CreateOrder(command);

        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Value);
        Assert.Null(response.Error);
    }

    [Fact]
    public void Controller_InvalidOrder_Returns400()
    {
        var handler = new CreateOrderHandler();
        var controller = new OrderController(handler);

        var command = new CreateOrderCommand("CUST-1", []);

        var response = controller.CreateOrder(command);

        Assert.Equal(400, response.StatusCode);
        Assert.Null(response.Value);
        Assert.Contains("at least one item", response.Error);
    }
}
