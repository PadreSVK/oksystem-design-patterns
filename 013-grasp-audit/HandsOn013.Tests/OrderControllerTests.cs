// =============================================================================
// Hands-On 013 — GRASP Audit Tests
//
// These tests are always compiled and run.
// Fix OrderController.cs in HandsOn013.GraspAudit/ until all pass.
// =============================================================================

using Xunit;
using HandsOn013.GraspAudit;

namespace HandsOn013.Tests;

public class OrderControllerTests
{
    // ── Simple test doubles (no NSubstitute needed) ──────────────────────────

    private sealed class StubInventoryService(bool available) : IInventoryService
    {
        public bool IsStockAvailable(Order order) => available;
    }

    private sealed class SpyOrderRepository : IOrderRepository
    {
        public int SaveCallCount { get; private set; }
        public Order? LastSaved { get; private set; }

        public void Save(Order order)
        {
            SaveCallCount++;
            LastSaved = order;
        }
    }

    private sealed class SpyEmailService : IEmailService
    {
        public int SendCallCount { get; private set; }
        public Order? LastOrder { get; private set; }

        public void SendConfirmation(Order order)
        {
            SendCallCount++;
            LastOrder = order;
        }
    }

    // ── Order.CalculateTotal (Information Expert) ────────────────────────────

    [Fact]
    public void Order_CalculateTotal_SumsLineItemPrices()
    {
        var order = new Order
        {
            Lines =
            [
                new() { ProductId = 1, Price = 10m, Quantity = 3 },
                new() { ProductId = 2, Price = 20m, Quantity = 2 },
            ]
        };

        order.CalculateTotal();

        Assert.Equal(70m, order.TotalAmount);
    }

    [Fact]
    public void Order_CalculateTotal_EmptyLines_ReturnsZero()
    {
        var order = new Order { Lines = [] };

        order.CalculateTotal();

        Assert.Equal(0m, order.TotalAmount);
    }

    [Fact]
    public void Order_CalculateTotal_SingleLine()
    {
        var order = new Order
        {
            Lines = [new() { ProductId = 1, Price = 99.99m, Quantity = 1 }]
        };

        order.CalculateTotal();

        Assert.Equal(99.99m, order.TotalAmount);
    }

    // ── OrderController.Checkout (Controller + delegation) ───────────────────

    [Fact]
    public void Checkout_ValidOrder_CalculatesTotalAndSavesAndSendsEmail()
    {
        var repo = new SpyOrderRepository();
        var email = new SpyEmailService();
        var inventory = new StubInventoryService(available: true);
        var controller = new OrderController(repo, email, inventory);

        var order = new Order
        {
            Id = 1,
            CustomerEmail = "test@example.com",
            Lines =
            [
                new() { ProductId = 101, Price = 100m, Quantity = 2 },
                new() { ProductId = 102, Price = 50m, Quantity = 1 },
            ]
        };

        controller.Checkout(order);

        Assert.Equal(250m, order.TotalAmount);
        Assert.Equal(1, repo.SaveCallCount);
        Assert.Same(order, repo.LastSaved);
        Assert.Equal(1, email.SendCallCount);
        Assert.Same(order, email.LastOrder);
    }

    [Fact]
    public void Checkout_StockUnavailable_Throws()
    {
        var repo = new SpyOrderRepository();
        var email = new SpyEmailService();
        var inventory = new StubInventoryService(available: false);
        var controller = new OrderController(repo, email, inventory);

        var order = new Order
        {
            Id = 2,
            Lines = [new() { ProductId = 1, Price = 10m, Quantity = 1 }]
        };

        Assert.Throws<InvalidOperationException>(() => controller.Checkout(order));
        Assert.Equal(0, repo.SaveCallCount);
        Assert.Equal(0, email.SendCallCount);
    }

    [Fact]
    public void Checkout_StockUnavailable_DoesNotCalculateTotal()
    {
        var inventory = new StubInventoryService(available: false);
        var controller = new OrderController(new SpyOrderRepository(), new SpyEmailService(), inventory);

        var order = new Order
        {
            Lines = [new() { ProductId = 1, Price = 50m, Quantity = 2 }]
        };

        Assert.Throws<InvalidOperationException>(() => controller.Checkout(order));
        Assert.Equal(0m, order.TotalAmount);
    }
}
