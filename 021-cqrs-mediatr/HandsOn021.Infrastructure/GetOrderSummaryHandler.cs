using HandsOn021.Application;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HandsOn021.Infrastructure;

// =============================================================================
// Task 2 — GetOrderSummaryHandler (Query Handler — Read Side)
// =============================================================================
//
// This is the QUERY handler — it reads from the denormalized read model.
// On the read side, it's OK to use DbContext directly (no repository needed)
// because there is no domain logic to protect.
//
// YOUR TASKS:
//   1. Implement IRequestHandler<GetOrderSummaryQuery, Result<OrderSummaryDto>>.
//   2. Accept AppDbContext via constructor.
//   3. In Handle:
//      a. Query db.OrderSummaryViews with AsNoTracking()
//      b. Find the view matching query.OrderId
//      c. Map to OrderSummaryDto
//      d. Return Result<OrderSummaryDto>.Fail("...") if not found
//      e. Return Result<OrderSummaryDto>.Ok(dto) if found
//
// KEY INSIGHT: The query handler uses DbContext directly — this is the
//              pragmatic approach for the read side of CQRS. No repository
//              abstraction needed for simple projections.
//
// HINT: See the slide "CQRS — MediatR Query Handler (GetOrderSummary)"
// =============================================================================

public class GetOrderSummaryHandler
    // TODO: implement IRequestHandler<GetOrderSummaryQuery, Result<OrderSummaryDto>>
{
    // TODO: Accept AppDbContext via constructor

    // TODO: Implement Handle method
    //   1. db.OrderSummaryViews.AsNoTracking()
    //   2. .Where(v => v.OrderId == query.OrderId)
    //   3. .Select(v => new OrderSummaryDto(...))
    //   4. .FirstOrDefaultAsync(ct)
    //   5. Return Fail/Ok based on result
}
