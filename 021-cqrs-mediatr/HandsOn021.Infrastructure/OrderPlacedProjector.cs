using HandsOn021.Application;
using Mediator;

namespace HandsOn021.Infrastructure;

// =============================================================================
// OrderPlacedProjector — Shared (always compiled)
// =============================================================================
//
// The Projector listens for domain events (published via Mediator as
// INotification) and updates the denormalized read model.
// =============================================================================

public class OrderPlacedProjector(AppDbContext db)
    : INotificationHandler<OrderPlacedNotification>
{
    public async ValueTask Handle(OrderPlacedNotification notification, CancellationToken ct)
    {
        var summary = new OrderSummaryView
        {
            OrderId = notification.OrderId,
            Status = "Placed",
            TotalAmount = notification.TotalAmount,
            ItemCount = notification.Items.Count,
            PlacedAt = notification.PlacedAt
        };

        db.OrderSummaryViews.Add(summary);
        await db.SaveChangesAsync(ct);
    }
}
