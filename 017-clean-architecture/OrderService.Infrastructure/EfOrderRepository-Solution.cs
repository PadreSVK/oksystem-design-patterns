using Microsoft.EntityFrameworkCore;
using OrderService.Domain;

namespace OrderService.Infrastructure;

// =============================================================================
// Task 3 Solution — EfOrderRepository
// =============================================================================

public class EfOrderRepository(OrderDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task AddAsync(Order entity, CancellationToken ct = default)
    {
        await context.Orders.AddAsync(entity, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}
