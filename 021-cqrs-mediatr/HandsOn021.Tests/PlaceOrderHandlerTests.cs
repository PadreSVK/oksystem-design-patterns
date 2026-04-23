using HandsOn021.Application;
using HandsOn021.Domain;
using MediatR;
using Xunit;

namespace HandsOn021.Tests;

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

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            SaveCalled = true;
            return Task.CompletedTask;
        }
    }

    private class SpyPublisher : IPublisher
    {
        public List<INotification> Published { get; } = [];

        public Task Publish(object notification, CancellationToken ct = default)
        {
            if (notification is INotification n)
                Published.Add(n);
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken ct = default)
            where TNotification : INotification
        {
            Published.Add(notification);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Handle_creates_order_and_saves()
    {
        var repo = new FakeOrderRepository();
        var publisher = new SpyPublisher();
        var handler = new PlaceOrderHandler(repo, publisher);

        var command = new PlaceOrderCommand("CUST-1",
            [new OrderItemDto("Widget", 2, 10.00m)]);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repo.Added);
        Assert.True(repo.SaveCalled);
    }

    [Fact]
    public async Task Handle_returns_order_id()
    {
        var repo = new FakeOrderRepository();
        var publisher = new SpyPublisher();
        var handler = new PlaceOrderHandler(repo, publisher);

        var command = new PlaceOrderCommand("CUST-1",
            [new OrderItemDto("Widget", 1, 5.00m)]);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.Equal(repo.Added[0].Id, result.Value);
    }

    [Fact]
    public async Task Handle_publishes_OrderPlacedNotification()
    {
        var repo = new FakeOrderRepository();
        var publisher = new SpyPublisher();
        var handler = new PlaceOrderHandler(repo, publisher);

        var command = new PlaceOrderCommand("CUST-1",
        [
            new OrderItemDto("Widget", 2, 10.00m),
            new OrderItemDto("Gadget", 1, 25.00m)
        ]);

        await handler.Handle(command, CancellationToken.None);

        var notification = Assert.Single(publisher.Published);
        var orderPlaced = Assert.IsType<OrderPlacedNotification>(notification);
        Assert.Equal("CUST-1", orderPlaced.CustomerId);
        Assert.Equal(45.00m, orderPlaced.TotalAmount);
        Assert.Equal(2, orderPlaced.Items.Count);
    }

    [Fact]
    public async Task Handle_maps_items_correctly()
    {
        var repo = new FakeOrderRepository();
        var publisher = new SpyPublisher();
        var handler = new PlaceOrderHandler(repo, publisher);

        var command = new PlaceOrderCommand("CUST-1",
        [
            new OrderItemDto("Widget", 3, 7.50m),
            new OrderItemDto("Gadget", 1, 20.00m)
        ]);

        await handler.Handle(command, CancellationToken.None);

        var order = repo.Added[0];
        Assert.Equal(2, order.Items.Count);
        Assert.Contains(order.Items, i => i.ProductName == "Widget" && i.Quantity == 3);
        Assert.Contains(order.Items, i => i.ProductName == "Gadget" && i.UnitPrice == 20.00m);
    }
}
