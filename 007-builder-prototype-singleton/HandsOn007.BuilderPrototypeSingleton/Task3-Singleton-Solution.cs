// =============================================================================
// Hands-On 007 — Task 3: Thread-Safe Singleton  (SOLUTION)
// =============================================================================

using System.Collections.Concurrent;

namespace HandsOn.BuilderPrototypeSingleton;

public interface IMetricsRegistry
{
    void IncrementCounter(string name);
    IReadOnlyDictionary<string, long> GetSnapshot();
}

public sealed class MetricsRegistry : IMetricsRegistry
{
    // ✅ LazyThreadSafetyMode.ExecutionAndPublication: only one thread initialises,
    //    all others wait — prevents double-initialisation without double-check locking.
    private static readonly Lazy<MetricsRegistry> _lazy =
        new(() => new MetricsRegistry(), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly ConcurrentDictionary<string, long> _counters = new();

    private MetricsRegistry() { }   // ✅ private — nobody can call new MetricsRegistry()

    public static MetricsRegistry Instance => _lazy.Value;

    public void IncrementCounter(string name)
    {
        // AddOrUpdate with Interlocked.Increment on the stored ref is not straightforward
        // with ConcurrentDictionary. The correct approach: use GetOrAdd + Interlocked on
        // a boxed ref, or simply use AddOrUpdate with a lambda. For long values the lambda
        // approach is safe because ConcurrentDictionary guarantees update atomicity per key.
        _counters.AddOrUpdate(name, addValue: 1L, updateValueFactory: (_, old) => old + 1L);
    }

    public IReadOnlyDictionary<string, long> GetSnapshot() =>
        new Dictionary<string, long>(_counters);   // ✅ snapshot — callers cannot mutate _counters
}
