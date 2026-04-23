using HandsOn021.Domain;
using Microsoft.EntityFrameworkCore;

namespace HandsOn021.Infrastructure;

// =============================================================================
// EfOrderRepository — Shared (always compiled, pre-built from 020)
// =============================================================================

public class EfOrderRepository(AppDbContext db) : IOrderRepository
{
    public async Task<Order?> FindByIdWithItemsAsync(Guid id, CancellationToken ct = default) =>
        await db.Orders
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await db.Orders.AddAsync(order, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
