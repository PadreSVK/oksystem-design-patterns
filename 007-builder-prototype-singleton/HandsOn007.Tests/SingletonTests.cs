// =============================================================================
// Hands-On 007 — Task 3 Tests: MetricsRegistry Singleton
//
// These tests are always compiled and run.
// Fix Task3-Singleton.cs in HandsOn007.BuilderPrototypeSingleton/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.BuilderPrototypeSingleton;

namespace HandsOn007.Tests;

public class SingletonTests
{
    [Fact]
    public void Instance_CalledTwice_ReturnsSameReference()
    {
        var a = MetricsRegistry.Instance;
        var b = MetricsRegistry.Instance;
        Assert.Same(a, b);
    }

    [Fact]
    public void IncrementCounter_Sequential_CountsCorrectly()
    {
        var registry    = MetricsRegistry.Instance;
        var counterName = $"seq_{Guid.NewGuid()}";

        registry.IncrementCounter(counterName);
        registry.IncrementCounter(counterName);
        registry.IncrementCounter(counterName);

        Assert.Equal(3L, registry.GetSnapshot()[counterName]);
    }

    [Fact]
    public void IncrementCounter_Parallel1000_NoLostUpdates()
    {
        var registry    = MetricsRegistry.Instance;
        var counterName = $"par_{Guid.NewGuid()}";

        Parallel.For(0, 1000, _ => registry.IncrementCounter(counterName));

        Assert.Equal(1000L, registry.GetSnapshot()[counterName]);
    }

    [Fact]
    public void GetSnapshot_ReturnsIsolatedCopy_MutationDoesNotAffectRegistry()
    {
        var registry    = MetricsRegistry.Instance;
        var counterName = $"snap_{Guid.NewGuid()}";
        registry.IncrementCounter(counterName);

        var snapshot = (Dictionary<string, long>)registry.GetSnapshot();
        snapshot[counterName] = 999L;

        Assert.Equal(1L, registry.GetSnapshot()[counterName]);
    }
}
