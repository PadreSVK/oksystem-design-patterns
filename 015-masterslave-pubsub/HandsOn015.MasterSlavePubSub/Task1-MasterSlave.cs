namespace HandsOn.MasterSlavePubSub;

// ──── Types ──────────────────────────────────────────────────────────

public record ProcessedImage(string Path, long SizeBytes, TimeSpan Duration);

public interface IImageProcessor
{
    Task<ProcessedImage> ProcessAsync(string path, CancellationToken ct = default);
}

// ──── Master ─────────────────────────────────────────────────────────

public class ImageBatchProcessor
{
    private readonly IImageProcessor _processor;

    public ImageBatchProcessor(IImageProcessor processor)
    {
        _processor = processor;
    }

    /// <summary>
    /// Master: fan out each path to a slave task and aggregate results.
    /// </summary>
    public async Task<IReadOnlyList<ProcessedImage>> ProcessBatchAsync(
        IEnumerable<string> paths, CancellationToken ct = default)
    {
        // TODO Task 1 — fan out via Task.WhenAll, pass ct to each slave call
        // 1. Create a task per path by calling _processor.ProcessAsync(path, ct)
        // 2. Await all tasks with Task.WhenAll
        // 3. Return the results
        throw new NotImplementedException();
    }
}
