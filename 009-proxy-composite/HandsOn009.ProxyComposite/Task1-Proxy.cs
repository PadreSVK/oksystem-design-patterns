// =============================================================================
// Hands-On 009 — Task 1: Caching Proxy  (starter)
//
// Scenario: IProductRepository is slow (e.g. a database call). You need a
//           transparent caching layer that the caller never knows about.
//
// Your task:
//   Implement CachingProductProxy so that:
//   • GetByIdAsync — returns a cached copy on subsequent calls (same Guid)
//   • GetAllAsync  — always delegates to inner (list queries are not cached)
//   • SaveAsync    — delegates to inner AND removes any cached entry for that Id
//
// Proxy vs Decorator reminder:
//   Both wrap an interface.  The key difference is intent:
//   • Decorator  → adds behaviour (logging, metrics) — client controls wrapping
//   • Proxy      → controls access (lazy load, cache, auth) — often transparent
//
// Hint: use a plain Dictionary<Guid, Product> as the in-memory cache.
// =============================================================================

namespace HandsOn.ProxyComposite;

// ─── Domain ───────────────────────────────────────────────────────────────────

public sealed record Product(Guid Id, string Name, decimal Price);

// ─── Repository interface (subject) ──────────────────────────────────────────

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task SaveAsync(Product product, CancellationToken ct = default);
}

// ─── Real subject (provided — do not modify) ─────────────────────────────────

public sealed class SlowProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _store = [];

    /// <summary>Simulates a slow I/O operation (no actual delay in tests).</summary>
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

// ─── Caching proxy (your task) ────────────────────────────────────────────────

public sealed class CachingProductProxy : IProductRepository
{
    private readonly IProductRepository _inner;
    private readonly Dictionary<Guid, Product> _cache = [];

    public CachingProductProxy(IProductRepository inner) => _inner = inner;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // TODO: if _cache contains id, return cached value without hitting _inner
        // TODO: otherwise call _inner.GetByIdAsync, store result in _cache (if not null), return it
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        // TODO: delegate directly to _inner — list queries are never cached
        throw new NotImplementedException();
    }

    public async Task SaveAsync(Product product, CancellationToken ct = default)
    {
        // TODO: await _inner.SaveAsync(product, ct)
        // TODO: _cache.Remove(product.Id)  ← invalidate stale cached entry
        throw new NotImplementedException();
    }
}
