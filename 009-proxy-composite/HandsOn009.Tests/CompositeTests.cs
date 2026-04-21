// =============================================================================
// Hands-On 009 — Task 2 Tests: UI Component Tree (Composite)
//
// These tests are always compiled and run.
// Fix Task2-Composite.cs in HandsOn009.ProxyComposite/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.ProxyComposite;

namespace HandsOn009.Tests;

public class CompositeTests
{
    // ── Leaf rendering ────────────────────────────────────────────────────────

    [Fact]
    public void Button_Render_ProducesCorrectString()
    {
        var button = new Button("OK");
        var output = new List<string>();

        button.Render(output);

        Assert.Single(output);
        Assert.Equal("[Button] OK", output[0]);
    }

    [Fact]
    public void Label_Render_ProducesCorrectString()
    {
        var label = new Label("Title");
        var output = new List<string>();

        label.Render(output);

        Assert.Single(output);
        Assert.Equal("[Label] Title", output[0]);
    }

    [Fact]
    public void Leaf_Render_WithIndent_AddsLeadingSpaces()
    {
        var button = new Button("Submit");
        var output = new List<string>();

        button.Render(output, indent: 4);

        Assert.Equal("    [Button] Submit", output[0]);
    }

    // ── Panel (Composite) rendering ───────────────────────────────────────────

    [Fact]
    public void Panel_Render_EmptyPanel_ProducesSingleLine()
    {
        var panel = new Panel("Root");
        var output = new List<string>();

        panel.Render(output);

        Assert.Single(output);
        Assert.Equal("[Panel] Root", output[0]);
    }

    [Fact]
    public void Panel_Render_WithLeafChildren_ProducesIndentedTree()
    {
        var panel = new Panel("Form");
        panel.Add(new Label("Username"));
        panel.Add(new Button("Login"));
        var output = new List<string>();

        panel.Render(output);

        Assert.Equal(3, output.Count);
        Assert.Equal("[Panel] Form",         output[0]);
        Assert.Equal("  [Label] Username",   output[1]);
        Assert.Equal("  [Button] Login",     output[2]);
    }

    [Fact]
    public void Panel_Render_NestedPanels_ProducesDeepIndentation()
    {
        var root   = new Panel("Root");
        var inner  = new Panel("Inner");
        inner.Add(new Button("Click"));
        root.Add(inner);

        var output = new List<string>();
        root.Render(output);

        Assert.Equal(3, output.Count);
        Assert.Equal("[Panel] Root",      output[0]);
        Assert.Equal("  [Panel] Inner",   output[1]);
        Assert.Equal("    [Button] Click", output[2]);
    }

    [Fact]
    public void Panel_Render_WithIndentOffset_ShiftsWholeTree()
    {
        var panel = new Panel("P");
        panel.Add(new Label("L"));
        var output = new List<string>();

        panel.Render(output, indent: 4);

        Assert.Equal("    [Panel] P",   output[0]);
        Assert.Equal("      [Label] L", output[1]);
    }

    // ── Add / Remove ──────────────────────────────────────────────────────────

    [Fact]
    public void Panel_Add_IncreasesChildCount()
    {
        var panel = new Panel("P");
        panel.Add(new Button("B1"));
        panel.Add(new Button("B2"));
        var output = new List<string>();

        panel.Render(output);

        Assert.Equal(3, output.Count); // panel + 2 buttons
    }

    [Fact]
    public void Panel_Remove_ExcludesChildFromRender()
    {
        var panel  = new Panel("P");
        var button = new Button("Remove me");
        panel.Add(button);
        panel.Add(new Label("Keep me"));

        panel.Remove(button);

        var output = new List<string>();
        panel.Render(output);

        Assert.Equal(2, output.Count);
        Assert.DoesNotContain(output, l => l.Contains("Remove me"));
    }

    // ── GetAll (depth-first traversal) ────────────────────────────────────────

    [Fact]
    public void Button_GetAll_ReturnsSelf()
    {
        var button = new Button("B");
        var all = button.GetAll().ToList();

        Assert.Single(all);
        Assert.Same(button, all[0]);
    }

    [Fact]
    public void Panel_GetAll_ReturnsAllNodesDepthFirst()
    {
        var root   = new Panel("Root");
        var inner  = new Panel("Inner");
        var btn    = new Button("B");
        var lbl    = new Label("L");

        inner.Add(btn);
        root.Add(inner);
        root.Add(lbl);

        // Expected order (pre-order depth-first): Root → Inner → B → L
        var all = root.GetAll().Select(c => c.Name).ToList();

        Assert.Equal(["Root", "Inner", "B", "L"], all);
    }

    [Fact]
    public void Panel_GetAll_CountMatchesTreeSize()
    {
        var root = new Panel("Root");
        root.Add(new Button("B1"));
        root.Add(new Button("B2"));
        root.Add(new Label("L1"));

        Assert.Equal(4, root.GetAll().Count()); // root + 3 children
    }
}
