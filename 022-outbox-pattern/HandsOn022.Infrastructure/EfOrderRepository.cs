using HandsOn022.Domain;
using Microsoft.EntityFrameworkCore;

namespace HandsOn022.Infrastructure;

// =============================================================================
// EfOrderRepository — Shared (always compiled)
// =============================================================================

public class EfOrderRepository(AppDbContext db) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await db.Orders.AddAsync(order, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
