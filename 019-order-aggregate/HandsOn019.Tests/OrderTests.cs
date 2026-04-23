using HandsOn019.OrderAggregate;
using Xunit;

namespace HandsOn019.Tests;

public class OrderTests
{
    private static OrderItem MakeItem(string name = "Widget", int qty = 1, decimal price = 10.00m) =>
        OrderItem.Create(Guid.NewGuid(), name, qty, new Money(price, "USD"));

    // ── Place ────────────────────────────────────────────────────────

    [Fact]
    public void Place_ValidOrder_ReturnsOrderWithCorrectState()
    {
        var items = new[] { MakeItem("A", 2, 5.00m), MakeItem("B", 1, 3.00m) };

        var order = Order.Place("CUST-1", items);

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal("CUST-1", order.CustomerId);
        Assert.Equal(OrderStatus.Placed, order.Status);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(13.00m, order.Total.Amount); // 2*5 + 1*3
    }

    [Fact]
    public void Place_WithNoItems_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Order.Place("CUST-1", []));

        Assert.Contains("at least one item", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Place_WithBlankCustomerId_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Order.Place("", [MakeItem()]));

        Assert.Contains("CustomerId", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Place_ValidOrder_RaisesOrderPlacedEvent()
    {
        var order = Order.Place("CUST-1", [MakeItem()]);

        var evt = Assert.Single(order.DomainEvents);
        var placedEvent = Assert.IsType<OrderPlacedEvent>(evt);
        Assert.Equal(order.Id, placedEvent.OrderId);
        Assert.Equal("CUST-1", placedEvent.CustomerId);
    }

    // ── Cancel ───────────────────────────────────────────────────────

    [Fact]
    public void Cancel_PlacedOrder_SetsStatusCancelled()
    {
        var order = Order.Place("CUST-1", [MakeItem()]);
        order.ClearDomainEvents(); // clear the placed event

        order.Cancel("Changed my mind");

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_PlacedOrder_RaisesOrderCancelledEvent()
    {
        var order = Order.Place("CUST-1", [MakeItem()]);
        order.ClearDomainEvents();

        order.Cancel("Changed my mind");

        var evt = Assert.Single(order.DomainEvents);
        var cancelledEvent = Assert.IsType<OrderCancelledEvent>(evt);
        Assert.Equal(order.Id, cancelledEvent.OrderId);
        Assert.Equal("Changed my mind", cancelledEvent.Reason);
    }

    [Fact]
    public void Cancel_ShippedOrder_ThrowsDomainException()
    {
        var order = Order.Place("CUST-1", [MakeItem()]);
        // Use reflection to set Status to Shipped (simulating the lifecycle)
        typeof(Order).GetProperty(nameof(Order.Status))!
            .SetValue(order, OrderStatus.Shipped);

        var ex = Assert.Throws<DomainException>(() => order.Cancel("Too late"));

        Assert.Contains("shipped", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ── ClearDomainEvents ────────────────────────────────────────────

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var order = Order.Place("CUST-1", [MakeItem()]);
        Assert.NotEmpty(order.DomainEvents);

        order.ClearDomainEvents();

        Assert.Empty(order.DomainEvents);
    }
}
