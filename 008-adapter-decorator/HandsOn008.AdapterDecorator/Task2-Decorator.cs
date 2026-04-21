// =============================================================================
// Hands-On 008 — Task 2: Logging & Caching Decorator  (starter)
//
// Your tasks:
//
//   Task 2a — LoggingRepositoryDecorator<T>:
//     • Implements IRepository<T> — wraps any inner IRepository<T>
//     • Logs before and after each call: method name, entity type, duration (ms)
//     • GetAllAsync also logs the result count
//     • Delegates every call to _inner
//
//   Task 2b — CachingRepositoryDecorator<T>:
//     • Implements IRepository<T> where T : IEntity
//     • GetByIdAsync: check cache first; on miss fetch from inner + set cache (5 min TTL)
//     • GetAllAsync: not cached — delegate directly to inner
//     • SaveAsync: delegate to inner, then remove the cached entry for that Id
//
// Note: IEntity gives the decorator a stable way to get the entity's Id.
// =============================================================================

using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HandsOn.AdapterDecorator;

// ─── Marker interface — entities expose their Id ──────────────────────────────

public interface IEntity
{
    Guid Id { get; }
}

// ─── Domain type ──────────────────────────────────────────────────────────────

public sealed record Product(Guid Id, string Name, decimal Price) : IEntity;

// ─── Repository interface ─────────────────────────────────────────────────────

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task SaveAsync(T entity, CancellationToken ct = default);
}

// ─── Concrete implementation (provided — do not modify) ───────────────────────

public sealed class InMemoryProductRepository : IRepository<Product>
{
    private readonly Dictionary<Guid, Product> _store = [];

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Product>>([.. _store.Values]);

    public Task SaveAsync(Product entity, CancellationToken ct = default)
    {
        _store[entity.Id] = entity;
        return Task.CompletedTask;
    }
}

// ─── Logging decorator (your task — Task 2a) ─────────────────────────────────

public sealed class LoggingRepositoryDecorator<T>(
    IRepository<T> inner,
    ILogger<LoggingRepositoryDecorator<T>> logger) : IRepository<T>
{
    private readonly IRepository<T> _inner = inner;
    private readonly ILogger<LoggingRepositoryDecorator<T>> _logger = logger;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // TODO: start a Stopwatch (Stopwatch.StartNew())
        // TODO: logger.LogInformation("[Repo] GetById {Entity} {Id}", typeof(T).Name, id)
        // TODO: var result = await inner.GetByIdAsync(id, ct)
        // TODO: logger.LogInformation("[Repo] GetById {Entity} {Id} found={Found} duration={Ms}ms", ...)
        // TODO: return result
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        // TODO: log before, await inner, log count + duration, return
        throw new NotImplementedException();
    }

    public async Task SaveAsync(T entity, CancellationToken ct = default)
    {
        // TODO: log before, await inner, log duration
        throw new NotImplementedException();
    }
}

// ─── Caching decorator (your task — Task 2b) ─────────────────────────────────

public sealed class CachingRepositoryDecorator<T>(
    IRepository<T> inner,
    IMemoryCache cache) : IRepository<T>
    where T : IEntity
{
    private readonly IRepository<T> _inner = inner;
    private readonly IMemoryCache _cache = cache;

    private static string CacheKey(Guid id) => $"{typeof(T).Name}:{id}";

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // TODO: if (_cache.TryGetValue(CacheKey(id), out T? cached)) return cached
        // TODO: fetch from inner, store in cache with 5-minute absolute expiry, return
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => _inner.GetAllAsync(ct); // not cached — delegate directly

    public async Task SaveAsync(T entity, CancellationToken ct = default)
    {
        // TODO: await _inner.SaveAsync(entity, ct)
        // TODO: _cache.Remove(CacheKey(entity.Id))   ← invalidate stale entry
        throw new NotImplementedException();
    }
}
