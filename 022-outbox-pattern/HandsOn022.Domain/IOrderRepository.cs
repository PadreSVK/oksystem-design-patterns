namespace HandsOn022.Domain;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
