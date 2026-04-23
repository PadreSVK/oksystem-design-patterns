using HandsOn019.OrderAggregate;
using Xunit;

namespace HandsOn019.Tests;

public class OrderItemTests
{
    [Fact]
    public void Create_ValidInput_ReturnsOrderItem()
    {
        var item = OrderItem.Create(
            Guid.NewGuid(), "Widget", 3, new Money(10.00m, "USD"));

        Assert.Equal("Widget", item.Name);
        Assert.Equal(3, item.Quantity);
    }

    [Fact]
    public void Create_ZeroQuantity_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            OrderItem.Create(Guid.NewGuid(), "Widget", 0, new Money(5.00m, "USD")));

        Assert.Contains("positive", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_NegativeQuantity_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            OrderItem.Create(Guid.NewGuid(), "Widget", -1, new Money(5.00m, "USD")));

        Assert.Contains("positive", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_BlankName_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            OrderItem.Create(Guid.NewGuid(), "", 1, new Money(5.00m, "USD")));

        Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LineTotal_CalculatesCorrectly()
    {
        var item = OrderItem.Create(
            Guid.NewGuid(), "Widget", 3, new Money(10.00m, "USD"));

        Assert.Equal(30.00m, item.LineTotal.Amount);
        Assert.Equal("USD", item.LineTotal.Currency);
    }
}
