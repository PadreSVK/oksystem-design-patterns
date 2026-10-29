using Xunit;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using HandsOn.MasterSlavePubSub;

namespace HandsOn015.Tests;

public class PubSubTests
{
    // ── test doubles ─────────────────────────────────────────────────

    private sealed class StubOrderGateway : IOrderGateway
    {
        public Guid OrderIdToReturn { get; set; } = Guid.NewGuid();
        public string? ReceivedCustomerId { get; private set; }

        public Task<Guid> SaveOrderAsync(string customerId, CancellationToken ct)
        {
            ReceivedCustomerId = customerId;
            return Task.FromResult(OrderIdToReturn);
        }
    }

    private sealed class SpyInvoiceService : IInvoiceService
    {
        public List<(Guid OrderId, string CustomerId)> Calls { get; } = [];

        public Task GenerateAsync(Guid orderId, string customerId, CancellationToken ct)
        {
            Calls.Add((orderId, customerId));
            return Task.CompletedTask;
        }
    }

    private sealed class SpyStockService : IStockService
    {
        public List<Guid> Calls { get; } = [];

        public Task ReserveAsync(Guid orderId, CancellationToken ct)
        {
            Calls.Add(orderId);
            return Task.CompletedTask;
        }
    }

    // ── helper ───────────────────────────────────────────────────────

    private static (OrderService Svc, StubOrderGateway Gateway, SpyInvoiceService Invoice, SpyStockService Stock) BuildServices()
    {
        var gateway = new StubOrderGateway();
        var invoice = new SpyInvoiceService();
        var stock = new SpyStockService();

        var services = new ServiceCollection();
        services.AddMediator();
        services.AddSingleton<IOrderGateway>(gateway);
        services.AddSingleton<IInvoiceService>(invoice);
        services.AddSingleton<IStockService>(stock);
        services.AddTransient<OrderService>();

        var provider = services.BuildServiceProvider();
        var svc = provider.GetRequiredService<OrderService>();

        return (svc, gateway, invoice, stock);
    }

    // ── tests ────────────────────────────────────────────────────────

    [Fact]
    public async Task PlaceOrderAsync_SavesOrderViaGateway()
    {
        var (svc, gateway, _, _) = BuildServices();

        await svc.PlaceOrderAsync("CUST-1");

        Assert.Equal("CUST-1", gateway.ReceivedCustomerId);
    }

    [Fact]
    public async Task PlaceOrderAsync_PublishesNotification_BothHandlersInvoked()
    {
        var (svc, gateway, invoice, stock) = BuildServices();
        var expectedId = gateway.OrderIdToReturn;

        var orderId = await svc.PlaceOrderAsync("CUST-2");

        Assert.Equal(expectedId, orderId);

        // InvoiceHandler was called
        Assert.Single(invoice.Calls);
        Assert.Equal(expectedId, invoice.Calls[0].OrderId);
        Assert.Equal("CUST-2", invoice.Calls[0].CustomerId);

        // StockReservationHandler was called
        Assert.Single(stock.Calls);
        Assert.Equal(expectedId, stock.Calls[0]);
    }
}
