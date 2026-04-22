// =============================================================================
// Hands-On 011 — Task 2 Tests: Pipeline Behaviors (Mediator)
//
// These tests are always compiled and run.
// Fix Task2-PipelineBehavior.cs until all pass.
// =============================================================================

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using HandsOn.ChainMediatorIterator;

namespace HandsOn011.Tests;

public class PipelineBehaviorTests
{
    // ── LoggingBehavior ───────────────────────────────────────────────────────

    [Fact]
    public async Task LoggingBehavior_CallsNext_ReturnsResponse()
    {
        var store = new InMemoryProductStore();
        var product = new ProductDto(Guid.NewGuid(), "Anvil", 49.99m);
        store.Seed(product);

        var handler = new GetProductQueryHandler(store);
        var logging = new LoggingBehavior<GetProductQuery, ProductDto?>(
            NullLogger<LoggingBehavior<GetProductQuery, ProductDto?>>.Instance);

        var query = new GetProductQuery(product.Id);
        var result = await logging.HandleAsync(query, c => handler.HandleAsync(query, c));

        Assert.NotNull(result);
        Assert.Equal("Anvil", result.Name);
    }

    [Fact]
    public async Task LoggingBehavior_LogsBeforeAndAfter()
    {
        var store = new InMemoryProductStore();
        var handler = new GetProductQueryHandler(store);
        var capturingLogger = new CapturingLogger<LoggingBehavior<GetProductQuery, ProductDto?>>();
        var logging = new LoggingBehavior<GetProductQuery, ProductDto?>(capturingLogger);

        var query = new GetProductQuery(Guid.NewGuid());
        await logging.HandleAsync(query, c => handler.HandleAsync(query, c));

        Assert.Equal(2, capturingLogger.Messages.Count);
        Assert.Contains("Handling", capturingLogger.Messages[0]);
        Assert.Contains("Handled",  capturingLogger.Messages[1]);
    }

    // ── CachingBehavior ───────────────────────────────────────────────────────

    [Fact]
    public async Task CachingBehavior_FirstCall_HitsHandler()
    {
        var spy   = new SpyHandler();
        var cache = new Dictionary<string, object?>();
        var caching = new CachingBehavior<GetProductQuery, ProductDto?>(cache);

        var query = new GetProductQuery(Guid.NewGuid());
        await caching.HandleAsync(query, c => spy.HandleAsync(query, c));

        Assert.Equal(1, spy.CallCount);
    }

    [Fact]
    public async Task CachingBehavior_SecondCall_ServedFromCache()
    {
        var spy   = new SpyHandler();
        var cache = new Dictionary<string, object?>();
        var caching = new CachingBehavior<GetProductQuery, ProductDto?>(cache);

        var query = new GetProductQuery(Guid.NewGuid());
        await caching.HandleAsync(query, c => spy.HandleAsync(query, c)); // miss
        await caching.HandleAsync(query, c => spy.HandleAsync(query, c)); // hit

        Assert.Equal(1, spy.CallCount);
    }

    [Fact]
    public async Task CachingBehavior_DifferentKeys_BothHitHandler()
    {
        var spy   = new SpyHandler();
        var cache = new Dictionary<string, object?>();
        var caching = new CachingBehavior<GetProductQuery, ProductDto?>(cache);

        var q1 = new GetProductQuery(Guid.NewGuid());
        var q2 = new GetProductQuery(Guid.NewGuid());

        await caching.HandleAsync(q1, c => spy.HandleAsync(q1, c));
        await caching.HandleAsync(q2, c => spy.HandleAsync(q2, c));

        Assert.Equal(2, spy.CallCount);
    }

    // ── SimplePipeline (integration) ──────────────────────────────────────────

    [Fact]
    public async Task Pipeline_NoBehaviors_CallsHandlerDirectly()
    {
        var store = new InMemoryProductStore();
        var product = new ProductDto(Guid.NewGuid(), "Gear", 12m);
        store.Seed(product);

        var pipeline = new SimplePipeline<GetProductQuery, ProductDto?>(
            new GetProductQueryHandler(store), []);

        var result = await pipeline.SendAsync(new GetProductQuery(product.Id));

        Assert.NotNull(result);
        Assert.Equal("Gear", result.Name);
    }

    [Fact]
    public async Task Pipeline_LoggingAndCaching_SecondCallServedFromCache()
    {
        var store = new InMemoryProductStore();
        var product = new ProductDto(Guid.NewGuid(), "Widget", 9.99m);
        store.Seed(product);

        var spyHandler = new SpyGetProductHandler(store);
        var cache = new Dictionary<string, object?>();

        var pipeline = new SimplePipeline<GetProductQuery, ProductDto?>(
            spyHandler,
            [
                new LoggingBehavior<GetProductQuery, ProductDto?>(
                    NullLogger<LoggingBehavior<GetProductQuery, ProductDto?>>.Instance),
                new CachingBehavior<GetProductQuery, ProductDto?>(cache)
            ]);

        var query = new GetProductQuery(product.Id);
        var r1 = await pipeline.SendAsync(query); // miss → hits handler
        var r2 = await pipeline.SendAsync(query); // hit  → from cache

        Assert.NotNull(r1);
        Assert.NotNull(r2);
        Assert.Equal(1, spyHandler.CallCount); // handler called once only
    }

    // ── Test doubles ─────────────────────────────────────────────────────────

    private sealed class SpyHandler : IRequestHandler<GetProductQuery, ProductDto?>
    {
        public int CallCount { get; private set; }

        public Task<ProductDto?> HandleAsync(GetProductQuery request, CancellationToken ct = default)
        {
            CallCount++;
            return Task.FromResult<ProductDto?>(null);
        }
    }

    private sealed class SpyGetProductHandler(InMemoryProductStore store)
        : IRequestHandler<GetProductQuery, ProductDto?>
    {
        public int CallCount { get; private set; }

        public Task<ProductDto?> HandleAsync(GetProductQuery query, CancellationToken ct = default)
        {
            CallCount++;
            return Task.FromResult(store.Find(query.ProductId));
        }
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
            => Messages.Add(formatter(state, exception));
    }
}
