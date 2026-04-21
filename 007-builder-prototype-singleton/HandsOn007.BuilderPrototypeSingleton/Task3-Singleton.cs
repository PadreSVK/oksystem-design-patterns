// =============================================================================
// Hands-On 007 — Task 3: Thread-Safe Singleton  (starter)
//
// Requirements:
//   • MetricsRegistry must have exactly one instance (Lazy<T> pattern)
//   • IncrementCounter(string name) must be thread-safe (use Interlocked.Increment)
//   • GetSnapshot() returns an immutable copy — callers cannot mutate internal state
//   • IMetricsRegistry interface enables DI and test injection (stretch goal)
// =============================================================================

namespace HandsOn.BuilderPrototypeSingleton;

// ── Interface (used for DI + stretch goal) ────────────────────────────────────

public interface IMetricsRegistry
{
    void IncrementCounter(string name);
    IReadOnlyDictionary<string, long> GetSnapshot();
}

// ── Singleton implementation (your task) ─────────────────────────────────────

public sealed class MetricsRegistry : IMetricsRegistry
{
    // TODO: add a static Lazy<MetricsRegistry> field for thread-safe lazy initialisation
    //   private static readonly Lazy<MetricsRegistry> _lazy = new(..., LazyThreadSafetyMode.ExecutionAndPublication);

    // TODO: make the constructor private (prevents external instantiation)
    private MetricsRegistry() { }

    // TODO: expose a static Instance property that returns _lazy.Value
    public static MetricsRegistry Instance => throw new NotImplementedException();

    // TODO: store counters in a ConcurrentDictionary<string, long>

    public void IncrementCounter(string name)
    {
        // TODO: add counter with value 0 if absent, then atomically increment:
        //   Simpler: _counters.AddOrUpdate(name, _ => 1L, (_, v) => v + 1L);
        //   Thread-safe: see solution file for Interlocked.Increment approach.
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, long> GetSnapshot()
    {
        // TODO: return a snapshot (new Dictionary) so callers can't mutate internal state
        throw new NotImplementedException();
    }

    // ── Stretch goal ──────────────────────────────────────────────────────────
    // 1. Register: builder.Services.AddSingleton<IMetricsRegistry>(_ => MetricsRegistry.Instance);
    // 2. Inject IMetricsRegistry into a controller or service
    // 3. Verify the same instance is returned across multiple requests
}
