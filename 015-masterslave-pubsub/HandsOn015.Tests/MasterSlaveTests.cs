using System.Diagnostics;
using Xunit;
using HandsOn.MasterSlavePubSub;

namespace HandsOn015.Tests;

public class MasterSlaveTests
{
    // ── test double ──────────────────────────────────────────────────

    private sealed class FakeImageProcessor : IImageProcessor
    {
        public List<string> ProcessedPaths { get; } = [];
        public TimeSpan SimulatedDelay { get; set; } = TimeSpan.FromMilliseconds(50);

        public async Task<ProcessedImage> ProcessAsync(string path, CancellationToken ct)
        {
            await Task.Delay(SimulatedDelay, ct);
            ProcessedPaths.Add(path);
            return new ProcessedImage(path, 1024, SimulatedDelay);
        }
    }

    // ── tests ────────────────────────────────────────────────────────

    [Fact]
    public async Task ProcessBatchAsync_ProcessesAllImages()
    {
        var processor = new FakeImageProcessor();
        var batch = new ImageBatchProcessor(processor);
        var paths = new[] { "a.png", "b.png", "c.png" };

        var results = await batch.ProcessBatchAsync(paths);

        Assert.Equal(3, results.Count);
        Assert.All(paths, p => Assert.Contains(results, r => r.Path == p));
    }

    [Fact]
    public async Task ProcessBatchAsync_RunsInParallel()
    {
        var processor = new FakeImageProcessor { SimulatedDelay = TimeSpan.FromMilliseconds(200) };
        var batch = new ImageBatchProcessor(processor);
        var paths = Enumerable.Range(1, 5).Select(i => $"{i}.png");

        var sw = Stopwatch.StartNew();
        var results = await batch.ProcessBatchAsync(paths);
        sw.Stop();

        Assert.Equal(5, results.Count);
        // 5 × 200 ms sequential = 1000 ms; parallel should complete well under 800 ms
        Assert.True(sw.ElapsedMilliseconds < 800,
            $"Expected parallel execution < 800 ms, but took {sw.ElapsedMilliseconds} ms");
    }

    [Fact]
    public async Task ProcessBatchAsync_ForwardsCancellationToken()
    {
        var processor = new FakeImageProcessor { SimulatedDelay = TimeSpan.FromSeconds(5) };
        var batch = new ImageBatchProcessor(processor);
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => batch.ProcessBatchAsync(["slow.png"], cts.Token));
    }
}
