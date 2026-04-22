// =============================================================================
// Hands-On 012 — Task 2 Tests: Multi-Format Report Export (Visitor)
//
// These tests are always compiled and run.
// Fix Task2-Visitor.cs in HandsOn012.StateVisitor/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.StateVisitor;

namespace HandsOn012.Tests;

public class VisitorTests
{
    private static SalesReport BuildReport() => new(
        "Q1 Sales",
        [
            new LineItem("Widget",  100.00m),
            new LineItem("Gadget",  200.50m),
            new LineItem("Doohickey", 49.50m),
        ]);

    // ── CsvExportVisitor ──────────────────────────────────────────────────────

    [Fact]
    public void CsvVisitor_Result_ContainsHeaderRow()
    {
        var visitor = new CsvExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("Title,Description,Amount", visitor.Result);
    }

    [Fact]
    public void CsvVisitor_Result_ContainsReportTitle()
    {
        var visitor = new CsvExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("Q1 Sales", visitor.Result);
    }

    [Fact]
    public void CsvVisitor_Result_ContainsAllLineItemDescriptions()
    {
        var visitor = new CsvExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("Widget",    visitor.Result);
        Assert.Contains("Gadget",    visitor.Result);
        Assert.Contains("Doohickey", visitor.Result);
    }

    [Fact]
    public void CsvVisitor_Result_ContainsLineItemAmounts()
    {
        var visitor = new CsvExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("100.00", visitor.Result);
        Assert.Contains("200.50", visitor.Result);
        Assert.Contains("49.50",  visitor.Result);
    }

    [Fact]
    public void CsvVisitor_EmptyReport_ContainsOnlyHeader()
    {
        var report  = new SalesReport("Empty", []);
        var visitor = new CsvExportVisitor();
        report.Accept(visitor);

        Assert.Contains("Title,Description,Amount", visitor.Result);
        Assert.DoesNotContain("Widget", visitor.Result);
    }

    [Fact]
    public void CsvVisitor_TwoVisitors_ProduceIndependentResults()
    {
        var v1 = new CsvExportVisitor();
        var v2 = new CsvExportVisitor();
        var report = new SalesReport("R", [new LineItem("A", 1m)]);

        report.Accept(v1);
        // v2 not used — Result should not contain v1's data
        Assert.DoesNotContain("A", v2.Result);
    }

    // ── JsonExportVisitor ─────────────────────────────────────────────────────

    [Fact]
    public void JsonVisitor_Result_ContainsTitle()
    {
        var visitor = new JsonExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("\"title\":\"Q1 Sales\"", visitor.Result);
    }

    [Fact]
    public void JsonVisitor_Result_ContainsItemsArray()
    {
        var visitor = new JsonExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("\"items\":[", visitor.Result);
    }

    [Fact]
    public void JsonVisitor_Result_ContainsAllItemDescriptions()
    {
        var visitor = new JsonExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("Widget",    visitor.Result);
        Assert.Contains("Gadget",    visitor.Result);
        Assert.Contains("Doohickey", visitor.Result);
    }

    [Fact]
    public void JsonVisitor_Result_ContainsTotal()
    {
        var visitor = new JsonExportVisitor();
        BuildReport().Accept(visitor);

        // 100.00 + 200.50 + 49.50 = 350.00
        Assert.Contains("\"total\":350", visitor.Result);
    }

    [Fact]
    public void JsonVisitor_EmptyReport_HasEmptyItemsArray()
    {
        var report  = new SalesReport("Empty", []);
        var visitor = new JsonExportVisitor();
        report.Accept(visitor);

        Assert.Contains("\"items\":[]", visitor.Result);
        Assert.Contains("\"total\":0", visitor.Result);
    }

    // ── Visitor independence — same structure, different formats ──────────────

    [Fact]
    public void SameReport_CsvAndJson_ProduceDifferentFormats()
    {
        var csv  = new CsvExportVisitor();
        var json = new JsonExportVisitor();
        var report = BuildReport();

        report.Accept(csv);
        report.Accept(json);

        // Both contain the title, but in different formats
        Assert.Contains("Q1 Sales",           csv.Result);
        Assert.Contains("\"Q1 Sales\"",        json.Result);
        Assert.NotEqual(csv.Result, json.Result);
    }

    // ── XmlExportVisitor ──────────────────────────────────────────────────────

    [Fact]
    public void XmlVisitor_Result_HasReportRootElement()
    {
        var visitor = new XmlExportVisitor();
        BuildReport().Accept(visitor);

        Assert.StartsWith("<report>", visitor.Result);
        Assert.EndsWith("</report>", visitor.Result);
    }

    [Fact]
    public void XmlVisitor_Result_ContainsTitleElement()
    {
        var visitor = new XmlExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("<title>Q1 Sales</title>", visitor.Result);
    }

    [Fact]
    public void XmlVisitor_Result_ContainsItemsElement()
    {
        var visitor = new XmlExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("<items>", visitor.Result);
        Assert.Contains("</items>", visitor.Result);
    }

    [Fact]
    public void XmlVisitor_Result_ContainsAllLineItemDescriptions()
    {
        var visitor = new XmlExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("<description>Widget</description>",    visitor.Result);
        Assert.Contains("<description>Gadget</description>",    visitor.Result);
        Assert.Contains("<description>Doohickey</description>", visitor.Result);
    }

    [Fact]
    public void XmlVisitor_Result_ContainsLineItemAmounts()
    {
        var visitor = new XmlExportVisitor();
        BuildReport().Accept(visitor);

        Assert.Contains("<amount>100.00</amount>", visitor.Result);
        Assert.Contains("<amount>200.50</amount>", visitor.Result);
        Assert.Contains("<amount>49.50</amount>",  visitor.Result);
    }

    [Fact]
    public void XmlVisitor_EmptyReport_HasEmptyItemsElement()
    {
        var report  = new SalesReport("Empty", []);
        var visitor = new XmlExportVisitor();
        report.Accept(visitor);

        Assert.Contains("<items></items>", visitor.Result);
        Assert.DoesNotContain("<description>", visitor.Result);
    }

    [Fact]
    public void ThreeVisitors_SameReport_ProduceThreeDifferentFormats()
    {
        var csv  = new CsvExportVisitor();
        var json = new JsonExportVisitor();
        var xml  = new XmlExportVisitor();
        var report = BuildReport();

        report.Accept(csv);
        report.Accept(json);
        report.Accept(xml);

        Assert.NotEqual(csv.Result,  json.Result);
        Assert.NotEqual(csv.Result,  xml.Result);
        Assert.NotEqual(json.Result, xml.Result);
    }
}
