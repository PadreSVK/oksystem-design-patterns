// =============================================================================
// Hands-On 009 — Task 2: UI Component Tree with Composite  (solution)
// =============================================================================

namespace HandsOn.ProxyComposite;

public interface IUiComponent
{
    string Name { get; }
    void Render(IList<string> output, int indent = 0);
    IEnumerable<IUiComponent> GetAll();
}

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

// ✅ Composite — Panel treats leaves and nested panels uniformly
public sealed class Panel(string name) : IUiComponent
{
    private readonly List<IUiComponent> _children = [];

    public string Name { get; } = name;

    public void Add(IUiComponent component) => _children.Add(component);

    public void Remove(IUiComponent component) => _children.Remove(component);

    public void Render(IList<string> output, int indent = 0)
    {
        output.Add($"{new string(' ', indent)}[Panel] {Name}");

        foreach (var child in _children)
            child.Render(output, indent + 2);              // ✅ recursive delegation
    }

    public IEnumerable<IUiComponent> GetAll()
    {
        yield return this;                                  // ✅ pre-order: self first

        foreach (var child in _children)
            foreach (var item in child.GetAll())            // ✅ depth-first recursion
                yield return item;
    }
}
