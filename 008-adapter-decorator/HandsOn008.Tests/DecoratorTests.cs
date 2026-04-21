// =============================================================================
// Hands-On 008 — Task 2 Tests: Logging & Caching Decorator
//
// These tests are always compiled and run.
// Fix Task2-Decorator.cs in HandsOn008.AdapterDecorator/ until all pass.
// =============================================================================

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using HandsOn.AdapterDecorator;

namespace HandsOn008.Tests;

public class DecoratorTests
{
    // ── Logging decorator ─────────────────────────────────────────────────────

    [Fact]
    public async Task LoggingDecorator_GetById_DelegatesToInner()
    {
        var inner = new InMemoryProductRepository();
        var product = new Product(Guid.NewGuid(), "Widget", 9.99m);
        await inner.SaveAsync(product);

        var decorator = new LoggingRepositoryDecorator<Product>(
            inner, NullLogger<LoggingRepositoryDecorator<Product>>.Instance);

        var result = await decorator.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("Widget",   result.Name);
    }

    [Fact]
    public async Task LoggingDecorator_GetAll_DelegatesToInner()
    {
        var inner = new InMemoryProductRepository();
        await inner.SaveAsync(new Product(Guid.NewGuid(), "A", 1m));
        await inner.SaveAsync(new Product(Guid.NewGuid(), "B", 2m));

        var decorator = new LoggingRepositoryDecorator<Product>(
            inner, NullLogger<LoggingRepositoryDecorator<Product>>.Instance);

        var all = await decorator.GetAllAsync();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task LoggingDecorator_SaveAsync_DelegatesToInner()
    {
        var inner = new InMemoryProductRepository();
        var product = new Product(Guid.NewGuid(), "Gadget", 49.99m);

        var decorator = new LoggingRepositoryDecorator<Product>(
            inner, NullLogger<LoggingRepositoryDecorator<Product>>.Instance);

        await decorator.SaveAsync(product);

        // Verify the inner repository received the save
        var fromInner = await inner.GetByIdAsync(product.Id);
        Assert.NotNull(fromInner);
        Assert.Equal("Gadget", fromInner.Name);
    }

    [Fact]
    public async Task LoggingDecorator_GetById_ReturnsNullForMissingEntity()
    {
        var inner = new InMemoryProductRepository();
        var decorator = new LoggingRepositoryDecorator<Product>(
            inner, NullLogger<LoggingRepositoryDecorator<Product>>.Instance);

        var result = await decorator.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // ── Caching decorator ─────────────────────────────────────────────────────

    [Fact]
    public async Task CachingDecorator_SecondGetById_DoesNotHitInner()
    {
        var spy = new SpyRepository();
        var product = new Product(Guid.NewGuid(), "Cached", 5m);
        spy.Store(product);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var decorator = new CachingRepositoryDecorator<Product>(spy, cache);

        await decorator.GetByIdAsync(product.Id);   // cache miss — hits inner
        await decorator.GetByIdAsync(product.Id);   // cache hit — should NOT hit inner

        Assert.Equal(1, spy.GetByIdCallCount);
    }

    [Fact]
    public async Task CachingDecorator_SaveAsync_InvalidatesCacheEntry()
    {
        var spy = new SpyRepository();
        var product = new Product(Guid.NewGuid(), "ToInvalidate", 5m);
        spy.Store(product);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var decorator = new CachingRepositoryDecorator<Product>(spy, cache);

        await decorator.GetByIdAsync(product.Id);         // miss → inner (count = 1)

        var updated = product with { Name = "Updated" };
        await decorator.SaveAsync(updated);               // should invalidate cache

        await decorator.GetByIdAsync(product.Id);         // miss again → inner (count = 2)

        Assert.Equal(2, spy.GetByIdCallCount);
    }

    [Fact]
    public async Task CachingDecorator_GetAllAsync_AlwaysDelegatesToInner()
    {
        var spy = new SpyRepository();
        spy.Store(new Product(Guid.NewGuid(), "X", 1m));

        var cache = new MemoryCache(new MemoryCacheOptions());
        var decorator = new CachingRepositoryDecorator<Product>(spy, cache);

        var first  = await decorator.GetAllAsync();
        var second = await decorator.GetAllAsync();

        Assert.Single(first);
        Assert.Single(second);
        Assert.Equal(2, spy.GetAllCallCount); // always hits inner
    }

    [Fact]
    public async Task CachingDecorator_FirstGetById_ReturnCorrectEntity()
    {
        var spy = new SpyRepository();
        var product = new Product(Guid.NewGuid(), "Zap", 99m);
        spy.Store(product);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var decorator = new CachingRepositoryDecorator<Product>(spy, cache);

        var result = await decorator.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Zap", result.Name);
    }

    // ── Spy repository ────────────────────────────────────────────────────────

    private sealed class SpyRepository : IRepository<Product>
    {
        private readonly Dictionary<Guid, Product> _store = [];

        public int GetByIdCallCount { get; private set; }
        public int GetAllCallCount  { get; private set; }

        public void Store(Product p) => _store[p.Id] = p;

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            GetByIdCallCount++;
            return Task.FromResult(_store.GetValueOrDefault(id));
        }

        public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        {
            GetAllCallCount++;
            return Task.FromResult<IReadOnlyList<Product>>([.. _store.Values]);
        }

        public Task SaveAsync(Product entity, CancellationToken ct = default)
        {
            _store[entity.Id] = entity;
            return Task.CompletedTask;
        }
    }
}
