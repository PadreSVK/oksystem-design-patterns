using Microsoft.EntityFrameworkCore;
using OrderService.Domain;
using OrderService.Infrastructure;
using Xunit;

namespace HandsOn017.Tests;

/// <summary>
/// Tests for EfOrderRepository using EF Core InMemory provider.
/// </summary>
public class EfOrderRepositoryTests
{
    private static string _dbName = null!;

    private static OrderDbContext CreateContext(string? dbName = null)
    {
        _dbName = dbName ?? Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(_dbName)
            .Options;
        return new OrderDbContext(options);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_RoundTrips()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new EfOrderRepository(context);
        var order = Order.Create("CUST-1",
        [
            OrderLine.Create("PROD-A", 2, 9.99m)
        ]);

        // Act
        await repo.AddAsync(order);
        await repo.SaveChangesAsync();

        // Re-query with a fresh context to verify persistence
        await using var readContext = CreateContext(dbName);
        var readRepo = new EfOrderRepository(readContext);
        var loaded = await readRepo.GetByIdAsync(order.Id);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("CUST-1", loaded!.CustomerId);
        Assert.Single(loaded.Lines);
        Assert.Equal("PROD-A", loaded.Lines[0].ProductId);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_ReturnsNull()
    {
        await using var context = CreateContext();
        var repo = new EfOrderRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesLines()
    {
        await using var context = CreateContext();
        var repo = new EfOrderRepository(context);
        var order = Order.Create("CUST-2",
        [
            OrderLine.Create("PROD-A", 1, 5.00m),
            OrderLine.Create("PROD-B", 3, 2.50m)
        ]);

        await repo.AddAsync(order);
        await repo.SaveChangesAsync();

        var loaded = await repo.GetByIdAsync(order.Id);

        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.Lines.Count);
    }
}
