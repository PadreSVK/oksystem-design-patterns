// =============================================================================
// Hands-On 011 — Task 3: Iterators — Fibonacci & Async Paged Source  (starter)
//
// Your tasks:
//
//   Task 3a — FibonacciSequence.Generate():
//     • Returns an INFINITE IEnumerable<long> of Fibonacci numbers: 0, 1, 1, 2, 3, 5, 8, …
//     • Must use `yield return` — no List<T> is ever allocated.
//     • Callers consume a finite prefix via .Take(n).
//     • Hint: keep two variables (a = 0, b = 1).
//             Each step: yield a, then (a, b) = (b, a + b).
//
//   Task 3b — PagedDataSource<T>.GetAllAsync():
//     • Abstracts a paginated remote source behind a seamless IAsyncEnumerable<T>.
//     • Calls the provided fetchPage delegate with increasing page numbers.
//     • Yields every item from every page until HasMore == false.
//     • Must use `yield return` inside an async iterator.
//
// Iterator pattern reminder:
//   • IEnumerable<T> / IAsyncEnumerable<T> = Iterator interface (.NET built-in)
//   • `yield return` creates a state-machine iterator — execution is LAZY.
//   • The caller pulls values on demand; no intermediate collection is built.
// =============================================================================

using System.Runtime.CompilerServices;

namespace HandsOn.ChainMediatorIterator;

// ─── Task 3a: Infinite Fibonacci sequence ─────────────────────────────────────

public static class FibonacciSequence
{
    /// <summary>
    /// Lazily generates the infinite Fibonacci sequence starting at 0.
    /// Always consume with .Take(n) — this never terminates on its own.
    /// </summary>
    public static IEnumerable<long> Generate()
    {
        // TODO: long a = 0, b = 1;
        // TODO: while (true) { yield return a; (a, b) = (b, a + b); }
        throw new NotImplementedException();
        yield break; // keeps this an iterator method so it compiles as a stub
    }
}

// ─── Task 3b: Paginated async data source ─────────────────────────────────────

public sealed record Page<T>(IReadOnlyList<T> Items, int PageNumber, bool HasMore);

public sealed class PagedDataSource<T>(
    Func<int, CancellationToken, Task<Page<T>>> fetchPage)
{
    private readonly Func<int, CancellationToken, Task<Page<T>>> _fetchPage = fetchPage;

    /// <summary>
    /// Streams all items across all pages as a seamless async sequence.
    /// Internally fetches pages one at a time; the caller just does `await foreach`.
    /// </summary>
    public async IAsyncEnumerable<T> GetAllAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        // TODO: int pageNumber = 1;
        // TODO: while (true)
        //       {
        //           var page = await _fetchPage(pageNumber, ct);
        //           foreach (var item in page.Items) yield return item;
        //           if (!page.HasMore) yield break;
        //           pageNumber++;
        //       }
        throw new NotImplementedException();
        yield break; // keeps this an async iterator method so it compiles as a stub
    }
}
