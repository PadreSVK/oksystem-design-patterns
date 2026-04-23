using System.Text.Json;
using HandsOn022.Domain;
using HandsOn022.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HandsOn022.Tests;

public class PlaceOrderHandlerTests
{
    private static AppDbContext CreateContext(string dbName) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options);

    [Fact]
    public async Task HandleAsync_saves_order_and_outbox_atomically()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var ctx = CreateContext(dbName))
        {
            var handler = new PlaceOrderHandler(ctx);
            var command = new PlaceOrderCommand("CUST-1",
                [new OrderItemDto("Widget", 2, 10.00m)]);

            await handler.HandleAsync(command);
        }

        using (var ctx = CreateContext(dbName))
        {
            Assert.Single(ctx.Orders);
            Assert.Single(ctx.OutboxMessages);
        }
    }

    [Fact]
    public async Task HandleAsync_outbox_message_has_correct_type()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var ctx = CreateContext(dbName))
        {
            var handler = new PlaceOrderHandler(ctx);
            var command = new PlaceOrderCommand("CUST-1",
                [new OrderItemDto("Widget", 1, 5.00m)]);

            await handler.HandleAsync(command);
        }

        using (var ctx = CreateContext(dbName))
        {
            var msg = await ctx.OutboxMessages.SingleAsync();
            Assert.Equal("OrderPlaced", msg.Type);
            Assert.False(msg.Sent);
        }
    }

    [Fact]
    public async Task HandleAsync_outbox_payload_contains_order_data()
    {
        var dbName = Guid.NewGuid().ToString();
        PlaceOrderResult result;

        using (var ctx = CreateContext(dbName))
        {
            var handler = new PlaceOrderHandler(ctx);
            var command = new PlaceOrderCommand("CUST-1",
            [
                new OrderItemDto("Widget", 2, 10.00m),
                new OrderItemDto("Gadget", 1, 25.00m)
            ]);

            result = await handler.HandleAsync(command);
        }

        using (var ctx = CreateContext(dbName))
        {
            var msg = await ctx.OutboxMessages.SingleAsync();
            var doc = JsonDocument.Parse(msg.Payload);
            var root = doc.RootElement;

            Assert.Equal(result.OrderId.ToString(), root.GetProperty("OrderId").GetString());
            Assert.Equal("CUST-1", root.GetProperty("CustomerId").GetString());
            Assert.Equal(45.00m, root.GetProperty("Total").GetDecimal());
        }
    }

    [Fact]
    public async Task HandleAsync_returns_correct_result()
    {
        using var ctx = CreateContext(Guid.NewGuid().ToString());
        var handler = new PlaceOrderHandler(ctx);

        var command = new PlaceOrderCommand("CUST-1",
        [
            new OrderItemDto("Widget", 2, 10.00m),
            new OrderItemDto("Gadget", 1, 25.00m)
        ]);

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.OrderId);
        Assert.Equal("CUST-1", result.CustomerId);
        Assert.Equal(2, result.ItemCount);
        Assert.Equal(45.00m, result.Total);
    }
}
