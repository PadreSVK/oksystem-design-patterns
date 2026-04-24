using OrderService.Application;
using OrderService.Domain;
using Xunit;

namespace HandsOn017.Tests;

/// <summary>
/// Tests for CreateOrderHandler — uses an in-memory fake repository.
/// </summary>
public class CreateOrderHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_CreatesOrderAndReturnsResponse()
    {
        // Arrange
        var repo = new FakeOrderRepository();
        var handler = new CreateOrderHandler(repo);
        var command = new CreateOrderCommand("CUST-1",
        [
            new CreateOrderLineDto("PROD-A", 2, 9.99m),
            new CreateOrderLineDto("PROD-B", 1, 4.50m)
        ]);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert
        Assert.Equal("CUST-1", response.CustomerId);
        Assert.Equal(2, response.LineCount);
        Assert.Equal(2 * 9.99m + 1 * 4.50m, response.Total);
        Assert.NotEqual(Guid.Empty, response.OrderId);

        // Verify persisted
        Assert.Single(repo.SavedOrders);
        Assert.True(repo.SaveChangesCalled);
    }

    [Fact]
    public async Task HandleAsync_EmptyLines_ThrowsArgumentException()
    {
        var repo = new FakeOrderRepository();
        var handler = new CreateOrderHandler(repo);
        var command = new CreateOrderCommand("CUST-1", []);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_BlankCustomerId_ThrowsArgumentException()
    {
        var repo = new FakeOrderRepository();
        var handler = new CreateOrderHandler(repo);
        var command = new CreateOrderCommand("",
        [
            new CreateOrderLineDto("PROD-A", 1, 5.00m)
        ]);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
    }

    // ── Fake repository ──────────────────────────────────────────────

    private sealed class FakeOrderRepository : IOrderRepository
    {
        public List<Order> SavedOrders { get; } = [];
        public bool SaveChangesCalled { get; private set; }

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var order = SavedOrders.FirstOrDefault(o => o.Id == id);
            return Task.FromResult(order);
        }

        public Task AddAsync(Order entity, CancellationToken ct = default)
        {
            SavedOrders.Add(entity);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            SaveChangesCalled = true;
            return Task.CompletedTask;
        }
    }
}
