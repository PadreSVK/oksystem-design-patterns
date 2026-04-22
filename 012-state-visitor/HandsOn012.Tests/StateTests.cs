// =============================================================================
// Hands-On 012 — Task 1 Tests: Order Lifecycle (State)
//
// These tests are always compiled and run.
// Fix Task1-State.cs in HandsOn012.StateVisitor/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.StateVisitor;

namespace HandsOn012.Tests;

public class StateTests
{
    // ── Initial state ─────────────────────────────────────────────────────────

    [Fact]
    public void NewOrder_IsInDraftState()
    {
        var order = new Order();
        Assert.Equal("Draft", order.StatusName);
    }

    // ── Valid transitions ─────────────────────────────────────────────────────

    [Fact]
    public void DraftOrder_Confirm_MovesToConfirmed()
    {
        var order = new Order();
        order.Confirm();
        Assert.Equal("Confirmed", order.StatusName);
    }

    [Fact]
    public void DraftOrder_Cancel_MovesToCancelled()
    {
        var order = new Order();
        order.Cancel();
        Assert.Equal("Cancelled", order.StatusName);
    }

    [Fact]
    public void ConfirmedOrder_Ship_MovesToShipped()
    {
        var order = new Order();
        order.Confirm();
        order.Ship();
        Assert.Equal("Shipped", order.StatusName);
    }

    [Fact]
    public void ConfirmedOrder_Cancel_MovesToCancelled()
    {
        var order = new Order();
        order.Confirm();
        order.Cancel();
        Assert.Equal("Cancelled", order.StatusName);
    }

    [Fact]
    public void DraftToConfirmedToShipped_FullHappyPath()
    {
        var order = new Order();
        order.Confirm();
        order.Ship();
        Assert.Equal("Shipped", order.StatusName);
    }

    // ── Invalid transitions — these pass even on starters (base class guards)

    [Fact]
    public void DraftOrder_Ship_ThrowsInvalidOperation()
    {
        var order = new Order();
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void ConfirmedOrder_Confirm_ThrowsInvalidOperation()
    {
        var order = new Order();
        order.Confirm();
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void ShippedOrder_Confirm_ThrowsInvalidOperation()
    {
        var order = new Order();
        order.Confirm();
        order.Ship();
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void ShippedOrder_Ship_ThrowsInvalidOperation()
    {
        var order = new Order();
        order.Confirm();
        order.Ship();
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void ShippedOrder_Cancel_ThrowsInvalidOperation()
    {
        var order = new Order();
        order.Confirm();
        order.Ship();
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void CancelledOrder_Confirm_ThrowsInvalidOperation()
    {
        var order = new Order();
        order.Cancel();
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void CancelledOrder_Ship_ThrowsInvalidOperation()
    {
        var order = new Order();
        order.Cancel();
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void CancelledOrder_Cancel_ThrowsInvalidOperation()
    {
        var order = new Order();
        order.Cancel();
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }
}
