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

    public async Task<IReadOnlyList<ProcessedImage>> ProcessBatchAsync(
        IEnumerable<string> paths, CancellationToken ct = default)
    {
        var tasks = paths.Select(path => _processor.ProcessAsync(path, ct));
        var results = await Task.WhenAll(tasks);
        return results;
    }
}
