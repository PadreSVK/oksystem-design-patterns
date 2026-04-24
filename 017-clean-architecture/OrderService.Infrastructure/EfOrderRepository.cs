using Microsoft.EntityFrameworkCore;
using OrderService.Domain;

namespace OrderService.Infrastructure;

// =============================================================================
// Task 3 — Implement EfOrderRepository in Infrastructure
// =============================================================================
//
// Infrastructure implements the interfaces defined in Domain using concrete
// technology (EF Core). This layer references Application (which references
// Domain), so it has access to IOrderRepository.
//
// YOUR TASKS:
//   1. Implement EfOrderRepository : IOrderRepository using OrderDbContext.
//   2. GetByIdAsync should eagerly load Lines (use Include).
//   3. AddAsync should call _context.Orders.AddAsync.
//   4. SaveChangesAsync should call _context.SaveChangesAsync.
//
// HINT: Use primary constructor to inject OrderDbContext.
// =============================================================================

public class EfOrderRepository
    // TODO: implement IOrderRepository
{
    // TODO: Accept OrderDbContext via constructor

    // TODO: Implement GetByIdAsync — use Include(o => o.Lines)

    // TODO: Implement AddAsync

    // TODO: Implement SaveChangesAsync
}
