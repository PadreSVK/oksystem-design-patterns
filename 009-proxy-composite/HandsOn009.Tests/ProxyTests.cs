// =============================================================================
// Hands-On 009 — Task 1 Tests: Caching Proxy
//
// These tests are always compiled and run.
// Fix Task1-Proxy.cs in HandsOn009.ProxyComposite/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.ProxyComposite;

namespace HandsOn009.Tests;

public class ProxyTests
{
    // ── Caching behaviour ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_FirstCall_HitsInnerRepository()
    {
        var spy = new SpyProductRepository();
        var product = new Product(Guid.NewGuid(), "Widget", 9.99m);
        spy.Store(product);

        var proxy = new CachingProductProxy(spy);

        var result = await proxy.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Widget", result.Name);
        Assert.Equal(1, spy.GetByIdCallCount);
    }

    [Fact]
    public async Task GetById_SecondCall_DoesNotHitInner()
    {
        var spy = new SpyProductRepository();
        var product = new Product(Guid.NewGuid(), "Gadget", 19.99m);
        spy.Store(product);

        var proxy = new CachingProductProxy(spy);

        await proxy.GetByIdAsync(product.Id);   // miss → hits inner (count = 1)
        await proxy.GetByIdAsync(product.Id);   // hit  → served from cache

        Assert.Equal(1, spy.GetByIdCallCount);
    }

    [Fact]
    public async Task GetById_MissingProduct_ReturnsNull()
    {
        var proxy = new CachingProductProxy(new SpyProductRepository());

        var result = await proxy.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_InvalidatesCachedEntry()
    {
        var spy = new SpyProductRepository();
        var product = new Product(Guid.NewGuid(), "OldName", 5m);
        spy.Store(product);

        var proxy = new CachingProductProxy(spy);

        await proxy.GetByIdAsync(product.Id);              // miss → inner (count = 1)

        var updated = product with { Name = "NewName" };
        spy.Store(updated);                                 // update the backing store
        await proxy.SaveAsync(updated);                    // should invalidate cache

        var result = await proxy.GetByIdAsync(product.Id); // miss again → inner (count = 2)

        Assert.Equal(2, spy.GetByIdCallCount);
        Assert.Equal("NewName", result?.Name);
    }

    [Fact]
    public async Task GetAll_AlwaysDelegatesToInner()
    {
        var spy = new SpyProductRepository();
        spy.Store(new Product(Guid.NewGuid(), "A", 1m));

        var proxy = new CachingProductProxy(spy);

        await proxy.GetAllAsync();
        await proxy.GetAllAsync();

        Assert.Equal(2, spy.GetAllCallCount);
    }

    [Fact]
    public async Task GetAll_ReturnsSavedProducts()
    {
        var inner = new SlowProductRepository();
        var p1 = new Product(Guid.NewGuid(), "Alpha", 1m);
        var p2 = new Product(Guid.NewGuid(), "Beta",  2m);
        await inner.SaveAsync(p1);
        await inner.SaveAsync(p2);

        var proxy = new CachingProductProxy(inner);
        var all = await proxy.GetAllAsync();

        Assert.Equal(2, all.Count);
    }

    // ── Spy / test double ─────────────────────────────────────────────────────

    private sealed class SpyProductRepository : IProductRepository
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

        public Task SaveAsync(Product product, CancellationToken ct = default)
        {
            _store[product.Id] = product;
            return Task.CompletedTask;
        }
    }
}
