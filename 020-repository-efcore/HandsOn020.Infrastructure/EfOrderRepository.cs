using HandsOn020.Domain;
using Microsoft.EntityFrameworkCore;

namespace HandsOn020.Infrastructure;

// =============================================================================
// Task 1 — Implement EfOrderRepository
// =============================================================================
//
// Implement the IOrderRepository interface using EF Core and AppDbContext.
// The repository encapsulates ALL EF Core details (Include, Where, OrderBy)
// so callers never see IQueryable<T>.
//
// YOUR TASKS:
//   1. Make EfOrderRepository implement IOrderRepository.
//   2. Accept AppDbContext via primary constructor.
//   3. Implement FindByIdWithItemsAsync:
//      - Use Include(o => o.Items) to eagerly load the navigation collection.
//      - Use SingleOrDefaultAsync to find by Id.
//   4. Implement AddAsync — use db.Orders.AddAsync.
//   5. Implement GetPlacedOrdersAsync:
//      - Return only orders with Status == OrderStatus.Placed.
//      - Include Items in the result.
//      - Return as IReadOnlyList<Order> (NOT IQueryable!).
//   6. Implement SaveChangesAsync — delegate to db.SaveChangesAsync.
//
// KEY INSIGHT: The repository returns domain-meaningful results (IReadOnlyList<T>,
//              T?, etc.) — NEVER IQueryable<T>. All EF Core query details stay
//              inside the repository implementation.
//
// HINT:
//   public async Task<Order?> FindByIdWithItemsAsync(Guid id, CancellationToken ct) =>
//       await db.Orders
//           .Include(o => o.Items)
//           .SingleOrDefaultAsync(o => o.Id == id, ct);
// =============================================================================

public class EfOrderRepository
    // TODO: implement IOrderRepository
{
    // TODO: Accept AppDbContext via primary constructor

    // TODO: FindByIdWithItemsAsync — Include(o => o.Items) + SingleOrDefaultAsync

    // TODO: AddAsync — db.Orders.AddAsync

    // TODO: GetPlacedOrdersAsync — Where(Status == Placed) + Include + ToListAsync

    // TODO: SaveChangesAsync — db.SaveChangesAsync
}
