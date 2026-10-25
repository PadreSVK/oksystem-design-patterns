// =============================================================================
// Hands-On 008 — Task 2: Logging & Caching Decorator  (solution)
// =============================================================================

using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HandsOn.AdapterDecorator;

public interface IEntity
{
    Guid Id { get; }
}

public sealed record Product(Guid Id, string Name, decimal Price) : IEntity;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task SaveAsync(T entity, CancellationToken ct = default);
}

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

// ✅ Logging decorator — adds timing + structured logging without modifying InMemoryProductRepository
public sealed class LoggingRepositoryDecorator<T>(
    IRepository<T> inner,
    ILogger<LoggingRepositoryDecorator<T>> logger) : IRepository<T>
{
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("[Repo] GetById {Entity} {Id}", typeof(T).Name, id);

        var result = await inner.GetByIdAsync(id, ct);

        sw.Stop();
        logger.LogInformation("[Repo] GetById {Entity} {Id} found={Found} duration={Ms}ms",
            typeof(T).Name, id, result is not null, sw.ElapsedMilliseconds);

        return result;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("[Repo] GetAll {Entity}", typeof(T).Name);

        var result = await inner.GetAllAsync(ct);

        sw.Stop();
        logger.LogInformation("[Repo] GetAll {Entity} count={Count} duration={Ms}ms",
            typeof(T).Name, result.Count, sw.ElapsedMilliseconds);

        return result;
    }

    public async Task SaveAsync(T entity, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("[Repo] Save {Entity}", typeof(T).Name);

        await inner.SaveAsync(entity, ct);

        sw.Stop();
        logger.LogInformation("[Repo] Save {Entity} duration={Ms}ms",
            typeof(T).Name, sw.ElapsedMilliseconds);
    }
}

// ✅ Caching decorator — transparent cache layer; caller never knows it exists
public sealed class CachingRepositoryDecorator<T>(
    IRepository<T> inner,
    IMemoryCache cache) : IRepository<T>
    where T : IEntity
{
    private static string CacheKey(Guid id) => $"{typeof(T).Name}:{id}";

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (cache.TryGetValue(CacheKey(id), out T? cached))
            return cached;                                      // ✅ cache hit

        var entity = await inner.GetByIdAsync(id, ct);         // ✅ cache miss — fetch

        if (entity is not null)
            cache.Set(CacheKey(id), entity, TimeSpan.FromMinutes(5));

        return entity;
    }

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => inner.GetAllAsync(ct); // list queries not cached — delegate directly

    public async Task SaveAsync(T entity, CancellationToken ct = default)
    {
        await inner.SaveAsync(entity, ct);
        cache.Remove(CacheKey(entity.Id));                      // ✅ invalidate stale entry
    }
}
