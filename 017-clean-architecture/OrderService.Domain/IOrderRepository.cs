namespace OrderService.Domain;

// =============================================================================
// Repository contracts — defined in Domain, implemented in Infrastructure.
// =============================================================================
//
// KEY INSIGHT: The Domain layer owns the INTERFACE. Infrastructure provides
// the IMPLEMENTATION. This is the Dependency Inversion Principle in action.
//
// Notice: Domain.csproj has ZERO PackageReference entries — no EF Core here!
// =============================================================================

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IOrderRepository : IRepository<Order>
{
}
