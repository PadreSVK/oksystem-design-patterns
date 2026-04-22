// =============================================================================
// Hands-On 012 — Task 2: Multi-Format Report Export with Visitor  (starter)
//
// Scenario: a SalesReport can be exported to different formats (CSV, JSON).
//           New formats must be addable without modifying the report model.
//
// Your tasks — implement the two concrete visitors:
//
//   Task 2a — CsvExportVisitor:
//     Visit(SalesReport) → add "Title,Description,Amount" header, then a title row
//     Visit(LineItem)    → add one CSV row: "{Description},{Amount:F2}"
//     Result             → all lines joined by newline
//
//   Task 2b — JsonExportVisitor:
//     Visit(SalesReport) → capture the title and compute total
//     Visit(LineItem)    → accumulate item JSON objects
//     Result             → emit {"title":"…","items":[…],"total":…}
//
//   Task 2c — XmlExportVisitor:
//     Visit(SalesReport) → capture the title
//     Visit(LineItem)    → accumulate item XML fragments
//     Result             → emit <report><title>…</title><items>…</items></report>
//
// Visitor pattern reminder:
//   • IReportVisitor       = Visitor interface
//   • SalesReport/LineItem = Elements (provided) — call Accept on themselves
//   • CsvExportVisitor     = Concrete Visitor A (your task)
//   • JsonExportVisitor    = Concrete Visitor B (your task)
//   • Caller drives export: report.Accept(visitor); var output = visitor.Result
// =============================================================================

namespace HandsOn.StateVisitor;

// ─── Element interface ────────────────────────────────────────────────────────

public interface IReportElement
{
    void Accept(IReportVisitor visitor);
}

// ─── Visitor interface ────────────────────────────────────────────────────────

public interface IReportVisitor
{
    void Visit(SalesReport report);
    void Visit(LineItem item);
}

// ─── Elements (provided — do not modify) ─────────────────────────────────────

public sealed record LineItem(string Description, decimal Amount) : IReportElement
{
    public void Accept(IReportVisitor visitor) => visitor.Visit(this);
}

public sealed class SalesReport(string title, IReadOnlyList<LineItem> items) : IReportElement
{
    public string Title { get; } = title;
    public IReadOnlyList<LineItem> Items { get; } = items;
    public decimal Total => Items.Sum(i => i.Amount);

    /// <summary>
    /// Visits the report header first, then each line item — the visitor sees
    /// the full tree without SalesReport knowing which format is being produced.
    /// </summary>
    public void Accept(IReportVisitor visitor)
    {
        visitor.Visit(this);           // visit report header
        foreach (var item in Items)
            item.Accept(visitor);      // visit each line item
    }
}

// ─── CSV visitor (your task — Task 2a) ───────────────────────────────────────

public sealed class CsvExportVisitor : IReportVisitor
{
    private readonly List<string> _lines = [];

    /// <summary>The complete CSV output after Accept has been called.</summary>
    public string Result => string.Join(Environment.NewLine, _lines);

    public void Visit(SalesReport report)
    {
        // TODO: _lines.Add("Title,Description,Amount")   ← header row
        // TODO: _lines.Add($"{report.Title},,{report.Total:F2}")  ← summary/title row
        throw new NotImplementedException();
    }

    public void Visit(LineItem item)
    {
        // TODO: _lines.Add($",{item.Description},{item.Amount:F2}")
        throw new NotImplementedException();
    }
}

// ─── JSON visitor (your task — Task 2b) ──────────────────────────────────────

public sealed class JsonExportVisitor : IReportVisitor
{
    private string _title = "";
    private decimal _total;
    private readonly List<string> _itemJsons = [];

    /// <summary>The complete JSON output after Accept has been called.</summary>
    public string Result =>
        $"{{\"title\":\"{_title}\",\"items\":[{string.Join(",", _itemJsons)}],\"total\":{_total}}}";

    public void Visit(SalesReport report)
    {
        // TODO: _title = report.Title
        // TODO: _total = report.Total
        throw new NotImplementedException();
    }

    public void Visit(LineItem item)
    {
        // TODO: _itemJsons.Add($"{{\"description\":\"{item.Description}\",\"amount\":{item.Amount}}}")
        throw new NotImplementedException();
    }
}

// ─── XML visitor (your task — Task 2c) ───────────────────────────────────────

public sealed class XmlExportVisitor : IReportVisitor
{
    private string _title = "";
    private readonly List<string> _itemXmls = [];

    /// <summary>The complete XML output after Accept has been called.</summary>
    public string Result =>
        $"<report><title>{_title}</title><items>{string.Join("", _itemXmls)}</items></report>";

    public void Visit(SalesReport report)
    {
        // TODO: _title = report.Title
        throw new NotImplementedException();
    }

    public void Visit(LineItem item)
    {
        // TODO: _itemXmls.Add($"<item><description>{item.Description}</description><amount>{item.Amount:F2}</amount></item>")
        throw new NotImplementedException();
    }
}
