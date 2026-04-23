using HandsOn019.OrderAggregate;
using Xunit;

namespace HandsOn019.Tests;

public class MoneyTests
{
    [Fact]
    public void Add_SameCurrency_ReturnsSummedAmount()
    {
        var a = new Money(10.00m, "USD");
        var b = new Money(5.50m, "USD");

        var result = a + b;

        Assert.Equal(15.50m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Add_DifferentCurrencies_ThrowsDomainException()
    {
        var usd = new Money(10.00m, "USD");
        var eur = new Money(5.00m, "EUR");

        var ex = Assert.Throws<DomainException>(() => usd + eur);
        Assert.Contains("different currencies", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Zero_HasZeroAmount_AndUsdCurrency()
    {
        Assert.Equal(0m, Money.Zero.Amount);
        Assert.Equal("USD", Money.Zero.Currency);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = new Money(9.99m, "USD");
        var b = new Money(9.99m, "USD");

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentAmount_AreNotEqual()
    {
        var a = new Money(9.99m, "USD");
        var b = new Money(10.00m, "USD");

        Assert.NotEqual(a, b);
    }
}
