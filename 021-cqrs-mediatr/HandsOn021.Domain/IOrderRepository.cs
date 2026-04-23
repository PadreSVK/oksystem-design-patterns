namespace HandsOn021.Domain;

public interface IOrderRepository
{
    Task<Order?> FindByIdWithItemsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
