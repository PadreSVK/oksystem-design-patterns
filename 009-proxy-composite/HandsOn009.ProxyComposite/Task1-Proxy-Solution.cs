// =============================================================================
// Hands-On 009 — Task 1: Caching Proxy  (solution)
// =============================================================================

namespace HandsOn.ProxyComposite;

public sealed record Product(Guid Id, string Name, decimal Price);

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task SaveAsync(Product product, CancellationToken ct = default);
}

public sealed class SlowProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _store = [];

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Product>>([.. _store.Values]);

    public Task SaveAsync(Product product, CancellationToken ct = default)
    {
        _store[product.Id] = product;
        return Task.CompletedTask;
    }
}

// ✅ Caching proxy — transparent to the caller, controls access to the real repo
public sealed class CachingProductProxy(IProductRepository inner) : IProductRepository
{
    private readonly Dictionary<Guid, Product> _cache = [];

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(id, out var cached))
            return cached;                                  // ✅ cache hit

        var product = await inner.GetByIdAsync(id, ct);    // ✅ cache miss

        if (product is not null)
            _cache[id] = product;

        return product;
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        => inner.GetAllAsync(ct);                          // list queries always fresh

    public async Task SaveAsync(Product product, CancellationToken ct = default)
    {
        await inner.SaveAsync(product, ct);
        _cache.Remove(product.Id);                         // ✅ invalidate stale entry
    }
}
