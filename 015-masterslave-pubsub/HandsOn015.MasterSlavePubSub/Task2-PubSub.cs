using MediatR;

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
        // TODO Task 2a — save order via _gateway, then publish OrderPlacedNotification
        // 1. Call _gateway.SaveOrderAsync to get the orderId
        // 2. Publish new OrderPlacedNotification(orderId, customerId) via _mediator
        // 3. Return the orderId
        throw new NotImplementedException();
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

    public async Task Handle(OrderPlacedNotification notification, CancellationToken ct)
    {
        // TODO Task 2b — call _invoiceService.GenerateAsync with orderId and customerId
        throw new NotImplementedException();
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

    public async Task Handle(OrderPlacedNotification notification, CancellationToken ct)
    {
        // TODO Task 2c — call _stockService.ReserveAsync with orderId
        throw new NotImplementedException();
    }
}
