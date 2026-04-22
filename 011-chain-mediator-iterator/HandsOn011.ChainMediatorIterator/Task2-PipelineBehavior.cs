// =============================================================================
// Hands-On 011 — Task 2: Logging & Caching Pipeline Behaviors  (starter)
//
// Scenario: a query pipeline (modelled after MediatR's IPipelineBehavior) lets
//           you add cross-cutting concerns — logging, caching, auth — without
//           touching the request handler.
//
// Your tasks:
//
//   Task 2a — LoggingBehavior<TRequest, TResponse>:
//     • Start a Stopwatch before calling next
//     • Log "[Pipeline] Handling {RequestType}" before
//     • Await next(ct) to call the inner handler / next behavior
//     • Log "[Pipeline] Handled {RequestType} in {ms}ms" after
//     • Return the response
//
//   Task 2b — CachingBehavior<TRequest, TResponse>:
//     • If the cache dictionary already contains request.CacheKey, return the cached value
//     • Otherwise call next(ct), store the result in the cache, return it
//
// Mediator / Pipeline pattern reminder:
//   • IPipelineBehavior<TRequest, TResponse> = Decorator-style pipeline step
//   • SimplePipeline (provided)              = assembles behaviors into a chain
//   • LoggingBehavior / CachingBehavior      = Concrete Behaviors (your task)
//   • IRequestHandler                        = the terminal handler at the end of the chain
// =============================================================================

using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace HandsOn.ChainMediatorIterator;

// ─── Custom pipeline types (mirror MediatR — no package required) ─────────────

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

// ─── Cacheable marker ─────────────────────────────────────────────────────────

public interface ICacheableRequest
{
    string CacheKey { get; }
}

// ─── Domain ───────────────────────────────────────────────────────────────────

public sealed record GetProductQuery(Guid ProductId) : ICacheableRequest
{
    public string CacheKey => $"product:{ProductId}";
}

public sealed record ProductDto(Guid Id, string Name, decimal Price);

// ─── Provided: in-memory product store ───────────────────────────────────────

public sealed class InMemoryProductStore
{
    private readonly Dictionary<Guid, ProductDto> _store = [];

    public void Seed(ProductDto product) => _store[product.Id] = product;
    public ProductDto? Find(Guid id) => _store.GetValueOrDefault(id);
}

// ─── Provided: terminal handler (do not modify) ───────────────────────────────

public sealed class GetProductQueryHandler(InMemoryProductStore store)
    : IRequestHandler<GetProductQuery, ProductDto?>
{
    public Task<ProductDto?> HandleAsync(GetProductQuery query, CancellationToken ct = default)
        => Task.FromResult(store.Find(query.ProductId));
}

// ─── Provided: pipeline assembler (do not modify) ────────────────────────────

public sealed class SimplePipeline<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> handler,
    IReadOnlyList<IPipelineBehavior<TRequest, TResponse>> behaviors)
{
    /// <summary>
    /// Sends the request through all behaviors in order, then to the handler.
    /// Behaviors[0] is the outermost wrapper; the last behavior wraps the handler.
    /// </summary>
    public Task<TResponse> SendAsync(TRequest request, CancellationToken ct = default)
    {
        RequestHandlerDelegate<TResponse> terminal = c => handler.HandleAsync(request, c);

        // Build the pipeline from the inside out: reverse so outer behaviors wrap inner ones
        var pipeline = behaviors
            .Reverse()
            .Aggregate(terminal, (next, behavior) =>
                c => behavior.HandleAsync(request, next, c));

        return pipeline(ct);
    }
}

// ─── Logging behavior (your task — Task 2a) ───────────────────────────────────

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next,
                                             CancellationToken ct = default)
    {
        // TODO: var sw = Stopwatch.StartNew()
        // TODO: _logger.LogInformation("[Pipeline] Handling {Request}", typeof(TRequest).Name)
        // TODO: var response = await next(ct)
        // TODO: sw.Stop()
        // TODO: _logger.LogInformation("[Pipeline] Handled {Request} in {Ms}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds)
        // TODO: return response
        throw new NotImplementedException();
    }
}

// ─── Caching behavior (your task — Task 2b) ───────────────────────────────────

public sealed class CachingBehavior<TRequest, TResponse>(Dictionary<string, object?> cache)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest
{
    private readonly Dictionary<string, object?> _cache = cache;

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next,
                                             CancellationToken ct = default)
    {
        // TODO: if (_cache.TryGetValue(request.CacheKey, out var cached))
        //           return (TResponse)cached!    ← cache hit
        // TODO: var response = await next(ct)   ← cache miss
        // TODO: _cache[request.CacheKey] = response
        // TODO: return response
        throw new NotImplementedException();
    }
}
