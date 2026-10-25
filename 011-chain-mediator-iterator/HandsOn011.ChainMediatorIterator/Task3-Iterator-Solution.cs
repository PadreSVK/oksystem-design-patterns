// =============================================================================
// Hands-On 011 — Task 3: Iterators — Fibonacci & Async Paged Source  (solution)
// =============================================================================

using System.Runtime.CompilerServices;

namespace HandsOn.ChainMediatorIterator;

public static class FibonacciSequence
{
    // ✅ Pure lazy iterator — no collection is ever allocated.
    //    `yield return` suspends execution; the caller pulls the next value on demand.
    public static IEnumerable<long> Generate()
    {
        long a = 0, b = 1;
        while (true)
        {
            yield return a;
            (a, b) = (b, a + b);
        }
    }
}

public sealed record Page<T>(IReadOnlyList<T> Items, int PageNumber, bool HasMore);

public sealed class PagedDataSource<T>(
    Func<int, CancellationToken, Task<Page<T>>> fetchPage)
{
    // ✅ Async iterator — callers `await foreach` a seamless stream;
    //    page boundaries are hidden behind the abstraction.
    public async IAsyncEnumerable<T> GetAllAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var pageNumber = 1;
        while (true)
        {
            var page = await fetchPage(pageNumber, ct);

            foreach (var item in page.Items)
                yield return item;

            if (!page.HasMore)
                yield break;

            pageNumber++;
        }
    }
}
