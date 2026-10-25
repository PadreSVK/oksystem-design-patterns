// =============================================================================
// Hands-On 012 — Task 2: Multi-Format Report Export with Visitor  (solution)
// =============================================================================

namespace HandsOn.StateVisitor;

public interface IReportElement
{
    void Accept(IReportVisitor visitor);
}

public interface IReportVisitor
{
    void Visit(SalesReport report);
    void Visit(LineItem item);
}

public sealed record LineItem(string Description, decimal Amount) : IReportElement
{
    public void Accept(IReportVisitor visitor) => visitor.Visit(this);
}

public sealed class SalesReport(string title, IReadOnlyList<LineItem> items) : IReportElement
{
    public string Title { get; } = title;
    public IReadOnlyList<LineItem> Items { get; } = items;
    public decimal Total => Items.Sum(i => i.Amount);

    public void Accept(IReportVisitor visitor)
    {
        visitor.Visit(this);
        foreach (var item in Items)
            item.Accept(visitor);
    }
}

// ✅ CSV visitor — caller does: report.Accept(csv); var text = csv.Result
public sealed class CsvExportVisitor : IReportVisitor
{
    private readonly List<string> _lines = [];

    public string Result => string.Join(Environment.NewLine, _lines);

    public void Visit(SalesReport report)
    {
        _lines.Add("Title,Description,Amount");
        _lines.Add($"{report.Title},,{report.Total:F2}");
    }

    public void Visit(LineItem item)
        => _lines.Add($",{item.Description},{item.Amount:F2}");
}

// ✅ JSON visitor — caller does: report.Accept(json); var text = json.Result
public sealed class JsonExportVisitor : IReportVisitor
{
    private string _title = "";
    private decimal _total;
    private readonly List<string> _itemJsons = [];

    public string Result =>
        $"{{\"title\":\"{_title}\",\"items\":[{string.Join(",", _itemJsons)}],\"total\":{_total}}}";

    public void Visit(SalesReport report)
    {
        _title = report.Title;
        _total = report.Total;
    }

    public void Visit(LineItem item)
        => _itemJsons.Add($"{{\"description\":\"{item.Description}\",\"amount\":{item.Amount}}}");
}

// ✅ XML visitor — caller does: report.Accept(xml); var text = xml.Result
public sealed class XmlExportVisitor : IReportVisitor
{
    private string _title = "";
    private readonly List<string> _itemXmls = [];

    public string Result =>
        $"<report><title>{_title}</title><items>{string.Join("", _itemXmls)}</items></report>";

    public void Visit(SalesReport report) => _title = report.Title;

    public void Visit(LineItem item)
        => _itemXmls.Add($"<item><description>{item.Description}</description><amount>{item.Amount:F2}</amount></item>");
}
