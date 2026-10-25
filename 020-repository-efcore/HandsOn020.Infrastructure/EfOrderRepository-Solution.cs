using HandsOn020.Domain;
using Microsoft.EntityFrameworkCore;

namespace HandsOn020.Infrastructure;

// =============================================================================
// Task 1 Solution — EfOrderRepository
// =============================================================================

public class EfOrderRepository(AppDbContext db) : IOrderRepository
{
    public async Task<Order?> FindByIdWithItemsAsync(Guid id, CancellationToken ct = default) =>
        await db.Orders
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await db.Orders.AddAsync(order, ct);

    public async Task<IReadOnlyList<Order>> GetPlacedOrdersAsync(CancellationToken ct = default) =>
        await db.Orders
            .Include(o => o.Items)
            .Where(o => o.Status == OrderStatus.Placed)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
