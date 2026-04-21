// =============================================================================
// Hands-On 009 — Task 2: UI Component Tree with Composite  (starter)
//
// Scenario: A UI toolkit needs to render a tree of widgets. A Panel contains
//           other components (children), while Button and Label are leaves.
//           All components share a common interface so the caller can treat
//           the entire tree uniformly.
//
// Your tasks:
//
//   Task 2a — Implement Panel.Render():
//     • Render this panel's own label at the current indent level
//     • Recursively render every child, increasing indent by 2 spaces
//
//   Task 2b — Implement Panel.Add() / Panel.Remove():
//     • Add appends the component to the internal list
//     • Remove removes the first matching component
//
//   Task 2c — Implement Panel.GetAll():
//     • Yield this panel itself, then recursively yield all descendants
//       (depth-first, pre-order)
//
// Composite pattern reminder:
//   • IUiComponent  = Component  (uniform interface for leaves and composites)
//   • Button, Label = Leaf       (no children, just render themselves)
//   • Panel         = Composite  (holds children, delegates rendering to them)
// =============================================================================

namespace HandsOn.ProxyComposite;

// ─── Component interface ──────────────────────────────────────────────────────

public interface IUiComponent
{
    string Name { get; }

    /// <summary>Renders the component to a list of indented strings.</summary>
    void Render(IList<string> output, int indent = 0);

    /// <summary>Enumerates this node and all descendants (depth-first, pre-order).</summary>
    IEnumerable<IUiComponent> GetAll();
}

// ─── Leaves (provided — do not modify) ───────────────────────────────────────

public sealed class Button(string name) : IUiComponent
{
    public string Name { get; } = name;

    public void Render(IList<string> output, int indent = 0)
        => output.Add($"{new string(' ', indent)}[Button] {Name}");

    public IEnumerable<IUiComponent> GetAll()
    {
        yield return this;
    }
}

public sealed class Label(string name) : IUiComponent
{
    public string Name { get; } = name;

    public void Render(IList<string> output, int indent = 0)
        => output.Add($"{new string(' ', indent)}[Label] {Name}");

    public IEnumerable<IUiComponent> GetAll()
    {
        yield return this;
    }
}

// ─── Composite (your task) ────────────────────────────────────────────────────

public sealed class Panel(string name) : IUiComponent
{
    private readonly List<IUiComponent> _children = [];

    public string Name { get; } = name;

    /// <summary>Adds a child component to this panel.</summary>
    public void Add(IUiComponent component)
    {
        // TODO: append component to _children
        throw new NotImplementedException();
    }

    /// <summary>Removes the first occurrence of the given component.</summary>
    public void Remove(IUiComponent component)
    {
        // TODO: remove component from _children
        throw new NotImplementedException();
    }

    public void Render(IList<string> output, int indent = 0)
    {
        // TODO: add "[Panel] {Name}" at current indent level
        // TODO: foreach child → child.Render(output, indent + 2)
        throw new NotImplementedException();
    }

    public IEnumerable<IUiComponent> GetAll()
    {
        // TODO: yield return this
        // TODO: foreach child in _children → foreach item in child.GetAll() → yield return item
        throw new NotImplementedException();
    }
}
