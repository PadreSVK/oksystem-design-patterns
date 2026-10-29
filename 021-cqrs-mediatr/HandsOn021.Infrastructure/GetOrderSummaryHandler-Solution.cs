using HandsOn021.Application;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace HandsOn021.Infrastructure;

// =============================================================================
// Task 2 Solution — GetOrderSummaryHandler (Query Handler)
// =============================================================================

public class GetOrderSummaryHandler(AppDbContext db)
    : IRequestHandler<GetOrderSummaryQuery, Result<OrderSummaryDto>>
{
    public async ValueTask<Result<OrderSummaryDto>> Handle(
        GetOrderSummaryQuery query, CancellationToken ct)
    {
        var dto = await db.OrderSummaryViews
            .AsNoTracking()
            .Where(v => v.OrderId == query.OrderId)
            .Select(v => new OrderSummaryDto(
                v.OrderId, v.Status, v.TotalAmount, v.ItemCount, v.PlacedAt))
            .FirstOrDefaultAsync(ct);

        return dto is null
            ? Result<OrderSummaryDto>.Fail($"Order {query.OrderId} not found.")
            : Result<OrderSummaryDto>.Ok(dto);
    }
}
