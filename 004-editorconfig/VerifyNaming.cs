// ============================================================
// Hands-On 004 — Verify .editorconfig naming rules
//
// Run the program:
//   dotnet run
//
// Check style violations (after you've placed your .editorconfig):
//   dotnet format --verify-no-changes
//
// This file contains intentional naming violations.
// After you complete your .editorconfig (steps 2 & 3 in the exercise),
// IDE squiggles and/or dotnet format will report violations here.
//
// Your task:
//   1. Run the project — confirm it works (dotnet run)
//   2. Copy your completed .editorconfig to the repo root
//   3. Observe the squiggles / dotnet format output
//   4. Fix all naming violations in this file
//   5. Re-run dotnet format --verify-no-changes — confirm exit code 0
//   6. Re-run dotnet run — confirm the output is unchanged
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

// -----------------------------------------------
// MetricsRegistry — contains TWO naming violations
// -----------------------------------------------

public sealed class MetricsRegistry
{
    // ❌ VIOLATION: private field must use _camelCase (prefix with _)
    //    Rule: dotnet_naming_rule.private_fields_rule
    private Dictionary<string, int> counters = new();

    // ❌ VIOLATION: constant must use PascalCase (not snake_case / all-caps)
    //    Rule: dotnet_naming_rule.constants_rule
    private const int max_entries = 100;

    public int TotalRecorded { get; private set; }

    public void Record(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (counters.Count >= max_entries)
            throw new InvalidOperationException(
                $"Registry is full — max {max_entries} distinct metric names allowed.");

        counters[name] = counters.TryGetValue(name, out var current) ? current + 1 : 1;
        TotalRecorded++;
    }

    public IReadOnlyDictionary<string, int> GetSnapshot() => counters;
}
