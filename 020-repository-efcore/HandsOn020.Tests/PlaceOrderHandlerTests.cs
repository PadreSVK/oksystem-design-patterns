using HandsOn020.Application;
using HandsOn020.Domain;
using Xunit;

namespace HandsOn020.Tests;

public class PlaceOrderHandlerTests
{
    private class FakeOrderRepository : IOrderRepository
    {
        public List<Order> Added { get; } = [];
        public bool SaveCalled { get; private set; }

        public Task<Order?> FindByIdWithItemsAsync(Guid id, CancellationToken ct = default) =>
            Task.FromResult(Added.FirstOrDefault(o => o.Id == id));

        public Task AddAsync(Order order, CancellationToken ct = default)
        {
            Added.Add(order);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Order>> GetPlacedOrdersAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Order>>(
                Added.Where(o => o.Status == OrderStatus.Placed).ToList());

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            SaveCalled = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task HandleAsync_adds_order_to_repository()
    {
        var repo = new FakeOrderRepository();
        var handler = new PlaceOrderHandler(repo);

        var command = new PlaceOrderCommand("CUST-1",
            [new OrderItemDto("Widget", 2, 10.00m)]);

        await handler.HandleAsync(command);

        Assert.Single(repo.Added);
        Assert.Equal("CUST-1", repo.Added[0].CustomerId);
    }

    [Fact]
    public async Task HandleAsync_saves_changes()
    {
        var repo = new FakeOrderRepository();
        var handler = new PlaceOrderHandler(repo);

        var command = new PlaceOrderCommand("CUST-1",
            [new OrderItemDto("Widget", 1, 5.00m)]);

        await handler.HandleAsync(command);

        Assert.True(repo.SaveCalled);
    }

    [Fact]
    public async Task HandleAsync_returns_correct_result()
    {
        var repo = new FakeOrderRepository();
        var handler = new PlaceOrderHandler(repo);

        var command = new PlaceOrderCommand("CUST-1",
        [
            new OrderItemDto("Widget", 2, 10.00m),
            new OrderItemDto("Gadget", 1, 25.00m)
        ]);

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.OrderId);
        Assert.Equal("CUST-1", result.CustomerId);
        Assert.Equal(2, result.ItemCount);
        Assert.Equal(45.00m, result.Total); // 2*10 + 1*25
    }

    [Fact]
    public async Task HandleAsync_maps_items_correctly()
    {
        var repo = new FakeOrderRepository();
        var handler = new PlaceOrderHandler(repo);

        var command = new PlaceOrderCommand("CUST-1",
        [
            new OrderItemDto("Widget", 3, 7.50m),
            new OrderItemDto("Gadget", 1, 20.00m)
        ]);

        await handler.HandleAsync(command);

        var order = repo.Added[0];
        Assert.Equal(2, order.Items.Count);
        Assert.Contains(order.Items, i => i.ProductName == "Widget" && i.Quantity == 3);
        Assert.Contains(order.Items, i => i.ProductName == "Gadget" && i.UnitPrice == 20.00m);
    }
}
