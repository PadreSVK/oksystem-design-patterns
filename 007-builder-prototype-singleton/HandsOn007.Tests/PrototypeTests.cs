// =============================================================================
// Hands-On 007 — Task 2 Tests: Prototype / Clone
//
// These tests are always compiled and run.
// Fix Task2-Prototype.cs in HandsOn007.BuilderPrototypeSingleton/ until all pass.
// Note: ShallowCloneBug deliberately passes — it exists to confirm the bug is real.
// =============================================================================

using Xunit;
using HandsOn.BuilderPrototypeSingleton;

namespace HandsOn007.Tests;

public class PrototypeTests
{
    [Fact]
    public void ShallowClone_AddToItems_MutatesOriginal()
    {
        // ❌ This confirms the shallow copy bug — expected to pass even before any fix.
        var original = new OrderTemplate { Name = "Base", Items = [new LineItem("A", 1)] };
        var copy     = original.ShallowClone();

        copy.Items.Add(new LineItem("B", 2));

        Assert.Equal(2, original.Items.Count);  // proves both share the same list
    }

    [Fact]
    public void DeepClone_AddToItems_DoesNotMutateOriginal()
    {
        var original = new OrderTemplate { Name = "Base", Items = [new LineItem("A", 1)] };
        var copy     = original.DeepClone();
        copy.Items.Add(new LineItem("B", 2));

        Assert.Single(original.Items);
        Assert.Equal(2, copy.Items.Count);
    }

    [Fact]
    public void ImmutableTemplate_AddItem_ReturnsNewInstance_OriginalUnchanged()
    {
        var original = ImmutableOrderTemplate.Empty("Base")
                           .AddItem(new LineItem("A", 1));
        var copy = original.AddItem(new LineItem("B", 2));

        Assert.Single(original.Items);
        Assert.Equal(2, copy.Items.Count);
        Assert.NotSame(original, copy);
    }

    [Fact]
    public void ImmutableTemplate_WithExpression_ProducesIndependentInstances()
    {
        var base1 = ImmutableOrderTemplate.Empty("Base")
                        .AddItem(new LineItem("SKU-001", 2));

        var q1 = base1 with { Name = "Q1 Batch" };
        var q2 = base1 with { Name = "Q2 Batch" };

        Assert.Equal("Base",     base1.Name);
        Assert.Equal("Q1 Batch", q1.Name);
        Assert.Equal("Q2 Batch", q2.Name);
        Assert.Same(base1.Items, q1.Items);  // ImmutableList safely shared
    }
}
