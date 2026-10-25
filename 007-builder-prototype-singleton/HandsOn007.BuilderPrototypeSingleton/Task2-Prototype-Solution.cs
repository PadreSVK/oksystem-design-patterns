// =============================================================================
// Hands-On 007 — Task 2: Deep vs. Shallow Clone  (SOLUTION)
// =============================================================================

using System.Collections.Immutable;

namespace HandsOn.BuilderPrototypeSingleton;

public sealed class LineItem(string sku, int quantity)
{
    public string Sku      { get; } = sku;
    public int    Quantity { get; } = quantity;
    public override string ToString() => $"{Sku} x{Quantity}";
}

public sealed class OrderTemplate
{
    public string         Name  { get; set; } = "";
    public List<LineItem> Items { get; set; } = [];

    public OrderTemplate ShallowClone() => (OrderTemplate)MemberwiseClone();

    /// <summary>✅ Deep copy — creates a new List so mutation is isolated.</summary>
    public OrderTemplate DeepClone() => new()
    {
        Name  = Name,
        Items = new List<LineItem>(Items)   // new list; LineItem itself is immutable
    };
}

/// <summary>
/// ✅ Immutable template — <see cref="ImmutableList{T}"/> makes mutation structurally
/// impossible; <c>record with {}</c> produces a non-destructive copy.
/// </summary>
public sealed record ImmutableOrderTemplate(string Name, ImmutableList<LineItem> Items)
{
    public static ImmutableOrderTemplate Empty(string name) =>
        new(name, ImmutableList<LineItem>.Empty);

    /// <summary>Returns a new template with <paramref name="item"/> appended.</summary>
    public ImmutableOrderTemplate AddItem(LineItem item) =>
        this with { Items = Items.Add(item) };
}

public static class PrototypeDemo
{
    public static void ShallowCloneBug()
    {
        var original = new OrderTemplate { Name = "Standard", Items = [new LineItem("SKU-001", 2)] };
        var copy     = original.ShallowClone();
        copy.Items.Add(new LineItem("SKU-002", 1));
        Console.WriteLine($"original.Items.Count = {original.Items.Count}");  // ❌ 2
        Console.WriteLine($"copy.Items.Count     = {copy.Items.Count}");
    }

    public static void DeepCloneFix()
    {
        var original = new OrderTemplate { Name = "Standard", Items = [new LineItem("SKU-001", 2)] };
        var copy     = original.DeepClone();
        copy.Items.Add(new LineItem("SKU-002", 1));
        Console.WriteLine($"original.Items.Count = {original.Items.Count}");  // ✅ still 1
        Console.WriteLine($"copy.Items.Count     = {copy.Items.Count}");
    }

    public static void ImmutableFix()
    {
        var original = ImmutableOrderTemplate.Empty("Standard")
                           .AddItem(new LineItem("SKU-001", 2));

        var copy = original.AddItem(new LineItem("SKU-002", 1));  // returns NEW instance
        Console.WriteLine($"original.Items.Count = {original.Items.Count}");  // ✅ 1
        Console.WriteLine($"copy.Items.Count     = {copy.Items.Count}");      // ✅ 2
    }
}
