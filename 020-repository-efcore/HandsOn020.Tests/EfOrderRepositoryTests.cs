using HandsOn020.Domain;
using HandsOn020.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HandsOn020.Tests;

public class EfOrderRepositoryTests
{
    private static AppDbContext CreateContext(string dbName) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options);

    private static Order CreateTestOrder(string customerId = "CUST-1") =>
        Order.Place(customerId, [OrderItem.Create("Widget", 2, 10.00m)]);

    [Fact]
    public async Task AddAsync_and_FindByIdWithItemsAsync_roundtrip()
    {
        var dbName = Guid.NewGuid().ToString();
        var order = CreateTestOrder();

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            await repo.AddAsync(order);
            await repo.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            var found = await repo.FindByIdWithItemsAsync(order.Id);

            Assert.NotNull(found);
            Assert.Equal(order.Id, found.Id);
            Assert.Equal("CUST-1", found.CustomerId);
            Assert.Equal(OrderStatus.Placed, found.Status);
        }
    }

    [Fact]
    public async Task FindByIdWithItemsAsync_returns_null_for_unknown_id()
    {
        using var ctx = CreateContext(Guid.NewGuid().ToString());
        var repo = new EfOrderRepository(ctx);

        var result = await repo.FindByIdWithItemsAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByIdWithItemsAsync_includes_items()
    {
        var dbName = Guid.NewGuid().ToString();
        var order = Order.Place("CUST-1",
        [
            OrderItem.Create("Widget", 1, 5.00m),
            OrderItem.Create("Gadget", 3, 15.00m)
        ]);

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            await repo.AddAsync(order);
            await repo.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            var found = await repo.FindByIdWithItemsAsync(order.Id);

            Assert.NotNull(found);
            Assert.Equal(2, found.Items.Count);
            Assert.Contains(found.Items, i => i.ProductName == "Widget");
            Assert.Contains(found.Items, i => i.ProductName == "Gadget");
        }
    }

    [Fact]
    public async Task GetPlacedOrdersAsync_returns_only_placed_orders()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);

            var placed = CreateTestOrder("CUST-1");
            var cancelled = CreateTestOrder("CUST-2");
            cancelled.Cancel();

            await repo.AddAsync(placed);
            await repo.AddAsync(cancelled);
            await repo.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            var results = await repo.GetPlacedOrdersAsync();

            Assert.Single(results);
            Assert.Equal("CUST-1", results[0].CustomerId);
        }
    }

    [Fact]
    public async Task GetPlacedOrdersAsync_includes_items()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            var order = Order.Place("CUST-1",
            [
                OrderItem.Create("Widget", 1, 5.00m),
                OrderItem.Create("Gadget", 3, 15.00m)
            ]);
            await repo.AddAsync(order);
            await repo.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            var results = await repo.GetPlacedOrdersAsync();

            Assert.Single(results);
            Assert.Equal(2, results[0].Items.Count);
        }
    }

    [Fact]
    public async Task SaveChangesAsync_persists_status_change()
    {
        var dbName = Guid.NewGuid().ToString();
        Guid orderId;

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            var order = CreateTestOrder();
            orderId = order.Id;
            await repo.AddAsync(order);
            await repo.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            var order = await repo.FindByIdWithItemsAsync(orderId);
            order!.Cancel();
            await repo.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var repo = new EfOrderRepository(ctx);
            var order = await repo.FindByIdWithItemsAsync(orderId);
            Assert.Equal(OrderStatus.Cancelled, order!.Status);
        }
    }
}
