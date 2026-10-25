// =============================================================================
// Hands-On 011 — Task 2: Logging & Caching Pipeline Behaviors  (solution)
// =============================================================================

using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace HandsOn.ChainMediatorIterator;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken ct = default);

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next,
                                CancellationToken ct = default);
}

public interface IRequestHandler<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}

public interface ICacheableRequest
{
    string CacheKey { get; }
}

public sealed record GetProductQuery(Guid ProductId) : ICacheableRequest
{
    public string CacheKey => $"product:{ProductId}";
}

public sealed record ProductDto(Guid Id, string Name, decimal Price);

public sealed class InMemoryProductStore
{
    private readonly Dictionary<Guid, ProductDto> _store = [];

    public void Seed(ProductDto product) => _store[product.Id] = product;
    public ProductDto? Find(Guid id) => _store.GetValueOrDefault(id);
}

public sealed class GetProductQueryHandler(InMemoryProductStore store)
    : IRequestHandler<GetProductQuery, ProductDto?>
{
    public Task<ProductDto?> HandleAsync(GetProductQuery query, CancellationToken ct = default)
        => Task.FromResult(store.Find(query.ProductId));
}

public sealed class SimplePipeline<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> handler,
    IReadOnlyList<IPipelineBehavior<TRequest, TResponse>> behaviors)
{
    public Task<TResponse> SendAsync(TRequest request, CancellationToken ct = default)
    {
        RequestHandlerDelegate<TResponse> terminal = c => handler.HandleAsync(request, c);

        var pipeline = behaviors
            .Reverse()
            .Aggregate(terminal, (next, behavior) =>
                c => behavior.HandleAsync(request, next, c));

        return pipeline(ct);
    }
}

// ✅ Logging behavior — transparent timing + structured logging around every request
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next,
                                             CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("[Pipeline] Handling {Request}", typeof(TRequest).Name);

        var response = await next(ct);

        sw.Stop();
        logger.LogInformation("[Pipeline] Handled {Request} in {Ms}ms",
            typeof(TRequest).Name, sw.ElapsedMilliseconds);

        return response;
    }
}

// ✅ Caching behavior — serves from cache on repeated queries; caller never knows
public sealed class CachingBehavior<TRequest, TResponse>(Dictionary<string, object?> cache)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest
{
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next,
                                             CancellationToken ct = default)
    {
        if (cache.TryGetValue(request.CacheKey, out var cached))
            return (TResponse)cached!;              // ✅ cache hit

        var response = await next(ct);              // ✅ cache miss — delegate to inner
        cache[request.CacheKey] = response;
        return response;
    }
}
