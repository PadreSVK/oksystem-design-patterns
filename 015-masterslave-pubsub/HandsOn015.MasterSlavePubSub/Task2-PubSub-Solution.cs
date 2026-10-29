using Mediator;

namespace HandsOn.MasterSlavePubSub;

// ──── Notification (event) ───────────────────────────────────────────

public record OrderPlacedNotification(Guid OrderId, string CustomerId) : INotification;

// ──── Service contracts ──────────────────────────────────────────────

public interface IOrderGateway
{
    Task<Guid> SaveOrderAsync(string customerId, CancellationToken ct = default);
}

public interface IInvoiceService
{
    Task GenerateAsync(Guid orderId, string customerId, CancellationToken ct = default);
}

public interface IStockService
{
    Task ReserveAsync(Guid orderId, CancellationToken ct = default);
}

// ──── Publisher ───────────────────────────────────────────────────────

public class OrderService
{
    private readonly IOrderGateway _gateway;
    private readonly IMediator _mediator;

    public OrderService(IOrderGateway gateway, IMediator mediator)
    {
        _gateway = gateway;
        _mediator = mediator;
    }

    public async Task<Guid> PlaceOrderAsync(string customerId, CancellationToken ct = default)
    {
        var orderId = await _gateway.SaveOrderAsync(customerId, ct);
        await _mediator.Publish(new OrderPlacedNotification(orderId, customerId), ct);
        return orderId;
    }
}

// ──── Subscriber: Invoice ────────────────────────────────────────────

public class InvoiceHandler : INotificationHandler<OrderPlacedNotification>
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async ValueTask Handle(OrderPlacedNotification notification, CancellationToken ct)
    {
        await _invoiceService.GenerateAsync(notification.OrderId, notification.CustomerId, ct);
    }
}

// ──── Subscriber: Stock Reservation ──────────────────────────────────

public class StockReservationHandler : INotificationHandler<OrderPlacedNotification>
{
    private readonly IStockService _stockService;

    public StockReservationHandler(IStockService stockService)
    {
        _stockService = stockService;
    }

    public async ValueTask Handle(OrderPlacedNotification notification, CancellationToken ct)
    {
        await _stockService.ReserveAsync(notification.OrderId, ct);
    }
}
