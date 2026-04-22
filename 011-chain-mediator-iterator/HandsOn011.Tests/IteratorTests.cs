// =============================================================================
// Hands-On 011 — Task 3 Tests: Iterators (Fibonacci & Async Paged Source)
//
// These tests are always compiled and run.
// Fix Task3-Iterator.cs until all pass.
// =============================================================================

using Xunit;
using HandsOn.ChainMediatorIterator;

namespace HandsOn011.Tests;

public class IteratorTests
{
    // ── Fibonacci ─────────────────────────────────────────────────────────────

    [Fact]
    public void Fibonacci_First10_MatchKnownSequence()
    {
        var expected = new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 };

        var actual = FibonacciSequence.Generate().Take(10).ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Fibonacci_Take0_ReturnsEmpty()
    {
        var result = FibonacciSequence.Generate().Take(0).ToArray();

        Assert.Empty(result);
    }

    [Fact]
    public void Fibonacci_Take1_Returns0()
    {
        var result = FibonacciSequence.Generate().Take(1).ToArray();

        Assert.Single(result);
        Assert.Equal(0L, result[0]);
    }

    [Fact]
    public void Fibonacci_IsLazy_CanTakeAnyCountWithoutException()
    {
        // Verifies the sequence does not pre-compute or allocate a list
        var result = FibonacciSequence.Generate().Take(1000).ToArray();

        Assert.Equal(1000, result.Length);
    }

    [Fact]
    public void Fibonacci_EachTermIsEqualToPreviousTwo()
    {
        var seq = FibonacciSequence.Generate().Take(20).ToArray();

        for (var i = 2; i < seq.Length; i++)
            Assert.Equal(seq[i - 2] + seq[i - 1], seq[i]);
    }

    [Fact]
    public void Fibonacci_CallingGenerateTwice_ReturnsFreshSequences()
    {
        // Two independent calls must each start from 0
        var s1 = FibonacciSequence.Generate().Take(5).ToArray();
        var s2 = FibonacciSequence.Generate().Take(5).ToArray();

        Assert.Equal(s1, s2);
    }

    // ── PagedDataSource ───────────────────────────────────────────────────────

    [Fact]
    public async Task PagedDataSource_SinglePage_ReturnsAllItems()
    {
        var source = new PagedDataSource<string>(
            (page, _) => Task.FromResult(new Page<string>(["a", "b", "c"], page, HasMore: false)));

        var all = await CollectAsync(source.GetAllAsync());

        Assert.Equal(["a", "b", "c"], all);
    }

    [Fact]
    public async Task PagedDataSource_MultiplePages_ReturnsAllItemsInOrder()
    {
        var pages = new Dictionary<int, Page<string>>
        {
            [1] = new(["a", "b"], 1, HasMore: true),
            [2] = new(["c", "d"], 2, HasMore: true),
            [3] = new(["e"],      3, HasMore: false),
        };

        var source = new PagedDataSource<string>(
            (page, _) => Task.FromResult(pages[page]));

        var all = await CollectAsync(source.GetAllAsync());

        Assert.Equal(["a", "b", "c", "d", "e"], all);
    }

    [Fact]
    public async Task PagedDataSource_EmptyFirstPage_ReturnsEmpty()
    {
        var source = new PagedDataSource<string>(
            (_, _) => Task.FromResult(new Page<string>([], 1, HasMore: false)));

        var all = await CollectAsync(source.GetAllAsync());

        Assert.Empty(all);
    }

    [Fact]
    public async Task PagedDataSource_FetchPageCalledForEachPage()
    {
        var callLog = new List<int>();

        var source = new PagedDataSource<int>((page, _) =>
        {
            callLog.Add(page);
            return Task.FromResult(new Page<int>([page * 10], page, HasMore: page < 3));
        });

        await CollectAsync(source.GetAllAsync());

        Assert.Equal([1, 2, 3], callLog);
    }

    [Fact]
    public async Task PagedDataSource_RespectsIntType()
    {
        var source = new PagedDataSource<int>(
            (page, _) => Task.FromResult(new Page<int>([1, 2, 3], page, HasMore: false)));

        var all = await CollectAsync(source.GetAllAsync());

        Assert.Equal([1, 2, 3], all);
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
            list.Add(item);
        return list;
    }
}
