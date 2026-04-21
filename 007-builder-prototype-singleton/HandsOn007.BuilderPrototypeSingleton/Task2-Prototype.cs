// =============================================================================
// Hands-On 007 — Task 2: Deep vs. Shallow Clone  (starter)
//
// Step 1 — run the ShallowCloneBug() demo below and observe the problem.
// Step 2 — understand WHY it fails (MemberwiseClone shares the List reference).
// Step 3 — implement ImmutableOrderTemplate using record + ImmutableList<LineItem>
//          so the bug is structurally impossible.
// =============================================================================

using System.Collections.Immutable;

namespace HandsOn.BuilderPrototypeSingleton;

// ── Domain types ──────────────────────────────────────────────────────────────

public sealed class LineItem(string sku, int quantity)
{
    public string Sku      { get; } = sku;
    public int    Quantity { get; } = quantity;

    public override string ToString() => $"{Sku} x{Quantity}";
}

// ── Mutable template with shallow-copy bug ────────────────────────────────────

public sealed class OrderTemplate
{
    public string          Name  { get; set; } = "";
    public List<LineItem>  Items { get; set; } = [];

    /// <summary>
    /// ❌ Shallow copy — <see cref="Items"/> list is shared between original and copy.
    ///    Adding to copy.Items also mutates the original. See the demo below.
    /// </summary>
    public OrderTemplate ShallowClone() => (OrderTemplate)MemberwiseClone();

    // TODO: add DeepClone() that creates a NEW List<LineItem> with the same elements
    //       (string and LineItem are already safe to share since LineItem is immutable)
    public OrderTemplate DeepClone() => throw new NotImplementedException();
}

// ── Immutable template (your task) ───────────────────────────────────────────

// TODO: implement ImmutableOrderTemplate as a sealed record:
//   • ImmutableList<LineItem> means Items can never be mutated in place
//   • Use `template with { Items = template.Items.Add(newItem) }` for non-destructive add
public sealed record ImmutableOrderTemplate(string Name, ImmutableList<LineItem> Items)
{
    public static ImmutableOrderTemplate Empty(string name) => throw new NotImplementedException();
    public ImmutableOrderTemplate AddItem(LineItem item) => throw new NotImplementedException();
}

// ── Demo (provided — read and understand, then run via tests) ─────────────────

public static class PrototypeDemo
{
    /// <summary>Demonstrates the shallow copy bug.</summary>
    public static void ShallowCloneBug()
    {
        var original = new OrderTemplate
        {
            Name  = "Standard",
            Items = [new LineItem("SKU-001", 2)]
        };

        var copy = original.ShallowClone();
        copy.Items.Add(new LineItem("SKU-002", 1));   // ← also mutates original!

        Console.WriteLine($"original.Items.Count = {original.Items.Count}");  // ❌ prints 2, not 1
        Console.WriteLine($"copy.Items.Count     = {copy.Items.Count}");
    }
}
