using Xunit;
using HandsOn.MvcMvvm;
using Microsoft.AspNetCore.Mvc;

namespace HandsOn014.Tests;

public class MvcTests
{
    // ── test double ──────────────────────────────────────────────────

    private sealed class StubOrderService : IOrderService
    {
        public CreateOrderCommand? ReceivedCommand { get; private set; }
        public ServiceResult<CreateOrderResponse> ResultToReturn { get; set; } = null!;

        public Task<ServiceResult<CreateOrderResponse>> CreateAsync(
            CreateOrderCommand command, CancellationToken ct)
        {
            ReceivedCommand = command;
            return Task.FromResult(ResultToReturn);
        }
    }

    // ── tests ────────────────────────────────────────────────────────

    [Fact]
    public async Task Post_ValidRequest_ReturnsCreatedAtAction_AndMapsCommand()
    {
        // Arrange
        var stub = new StubOrderService
        {
            ResultToReturn = ServiceResult<CreateOrderResponse>.Success(
                new CreateOrderResponse(Guid.NewGuid(), "C-1", 2))
        };
        var controller = new CreateOrderController(stub);
        var request = new CreateOrderRequest
        {
            CustomerId = "C-1",
            Lines =
            [
                new CreateOrderLineRequest(Guid.NewGuid(), 3),
                new CreateOrderLineRequest(Guid.NewGuid(), 5)
            ]
        };

        // Act
        var result = await controller.Post(request);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<CreateOrderResponse>(created.Value);
        Assert.Equal("C-1", response.CustomerId);
        Assert.Equal(2, response.LineCount);
        Assert.Equal(response.OrderId, created.RouteValues!["id"]);

        Assert.NotNull(stub.ReceivedCommand);
        Assert.Equal("C-1", stub.ReceivedCommand!.CustomerId);
        Assert.Equal(2, stub.ReceivedCommand.Lines.Count);
    }

    [Fact]
    public async Task Post_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var stub = new StubOrderService
        {
            ResultToReturn = ServiceResult<CreateOrderResponse>.Failure("out of stock")
        };
        var controller = new CreateOrderController(stub);
        var request = new CreateOrderRequest
        {
            CustomerId = "C-2",
            Lines = [new CreateOrderLineRequest(Guid.NewGuid(), 1)]
        };

        // Act
        var result = await controller.Post(request);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("out of stock", bad.Value);
    }

    [Fact]
    public async Task Post_InvalidModelState_ReturnsBadRequest_AndSkipsService()
    {
        // Arrange
        var stub = new StubOrderService();
        var controller = new CreateOrderController(stub);
        controller.ModelState.AddModelError("CustomerId", "Required");

        var request = new CreateOrderRequest { CustomerId = "", Lines = [] };

        // Act
        var result = await controller.Post(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Null(stub.ReceivedCommand); // service was never called
    }
}
