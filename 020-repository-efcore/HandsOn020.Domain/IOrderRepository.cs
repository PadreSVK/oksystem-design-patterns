namespace HandsOn020.Domain;

// =============================================================================
// Repository Interface — defined in Domain, implemented in Infrastructure
// =============================================================================
//
// KEY INSIGHT: The Domain layer owns the INTERFACE. Infrastructure provides
// the IMPLEMENTATION. This is the Dependency Inversion Principle in action.
//
// IMPORTANT: The repository returns domain-meaningful results (T?, IReadOnlyList<T>)
//            — NEVER IQueryable<T>. Returning IQueryable would leak infrastructure
//            details (Include, Where, OrderBy) into the Application layer.
// =============================================================================

public interface IOrderRepository
{
    Task<Order?> FindByIdWithItemsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetPlacedOrdersAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
