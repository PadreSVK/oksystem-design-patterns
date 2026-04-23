using HandsOn021.Application;
using HandsOn021.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HandsOn021.Tests;

public class GetOrderSummaryHandlerTests
{
    private static AppDbContext CreateContext(string dbName) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options);

    [Fact]
    public async Task Handle_returns_Ok_when_summary_exists()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var ctx = CreateContext(dbName))
        {
            ctx.OrderSummaryViews.Add(new OrderSummaryView
            {
                OrderId = Guid.Parse("aaaa0000-0000-0000-0000-000000000001"),
                Status = "Placed",
                TotalAmount = 45.00m,
                ItemCount = 2,
                PlacedAt = new DateTimeOffset(2025, 1, 15, 10, 0, 0, TimeSpan.Zero)
            });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var handler = new GetOrderSummaryHandler(ctx);
            var query = new GetOrderSummaryQuery(
                Guid.Parse("aaaa0000-0000-0000-0000-000000000001"));

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Placed", result.Value!.Status);
            Assert.Equal(45.00m, result.Value.TotalAmount);
            Assert.Equal(2, result.Value.ItemCount);
        }
    }

    [Fact]
    public async Task Handle_returns_Fail_when_summary_not_found()
    {
        using var ctx = CreateContext(Guid.NewGuid().ToString());
        var handler = new GetOrderSummaryHandler(ctx);

        var result = await handler.Handle(
            new GetOrderSummaryQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error);
    }

    [Fact]
    public async Task Handle_returns_correct_dto_fields()
    {
        var dbName = Guid.NewGuid().ToString();
        var orderId = Guid.NewGuid();
        var placedAt = new DateTimeOffset(2025, 6, 1, 12, 0, 0, TimeSpan.Zero);

        using (var ctx = CreateContext(dbName))
        {
            ctx.OrderSummaryViews.Add(new OrderSummaryView
            {
                OrderId = orderId,
                Status = "Placed",
                TotalAmount = 100.00m,
                ItemCount = 5,
                PlacedAt = placedAt
            });
            await ctx.SaveChangesAsync();
        }

        using (var ctx = CreateContext(dbName))
        {
            var handler = new GetOrderSummaryHandler(ctx);
            var result = await handler.Handle(
                new GetOrderSummaryQuery(orderId), CancellationToken.None);

            Assert.True(result.IsSuccess);
            var dto = result.Value!;
            Assert.Equal(orderId, dto.OrderId);
            Assert.Equal("Placed", dto.Status);
            Assert.Equal(100.00m, dto.TotalAmount);
            Assert.Equal(5, dto.ItemCount);
            Assert.Equal(placedAt, dto.PlacedAt);
        }
    }
}
