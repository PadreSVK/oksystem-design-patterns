// ============================================================
// Hands-On 004 — SOLUTION
// ============================================================

var registry = new MetricsRegistry();
registry.Record("orders.placed");
registry.Record("orders.placed");
registry.Record("payments.failed");
registry.Record("payments.failed");
registry.Record("payments.failed");

Console.WriteLine("Metrics snapshot:");
Console.WriteLine(new string('-', 40));
foreach (var (name, count) in registry.GetSnapshot())
    Console.WriteLine($"  {name,-30} {count,3}");
Console.WriteLine(new string('-', 40));
Console.WriteLine($"  Total events: {registry.TotalRecorded}");

public sealed class MetricsRegistry
{
    // ✅ FIXED: _counters — underscore + camelCase
    private Dictionary<string, int> _counters = new();

    // ✅ FIXED: MaxEntries — PascalCase constant
    private const int MaxEntries = 100;

    public int TotalRecorded { get; private set; }

    public void Record(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_counters.Count >= MaxEntries)
            throw new InvalidOperationException(
                $"Registry is full — max {MaxEntries} distinct metric names allowed.");

        _counters[name] = _counters.TryGetValue(name, out var current) ? current + 1 : 1;
        TotalRecorded++;
    }

    public IReadOnlyDictionary<string, int> GetSnapshot() => _counters;
}
