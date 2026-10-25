// =============================================================================
// Hands-On 010 — Task 3: Undo/Redo Text Editor with Command  (solution)
// =============================================================================

namespace HandsOn.StrategyObserverCommand;

public interface ICommand
{
    void Execute();
    void Undo();
}

public sealed class TextEditor
{
    public string Content { get; private set; } = string.Empty;

    public void InsertAt(int position, string text)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(position, Content.Length);
        Content = Content.Insert(position, text);
    }

    public void DeleteAt(int position, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(position + length, Content.Length);
        Content = Content.Remove(position, length);
    }
}

// ✅ Concrete commands — each knows how to execute and reverse itself
public sealed class InsertTextCommand(TextEditor editor, int position, string text) : ICommand
{
    public void Execute() => editor.InsertAt(position, text);
    public void Undo()    => editor.DeleteAt(position, text.Length);
}

public sealed class DeleteTextCommand(TextEditor editor, int position, int length) : ICommand
{
    private string _deletedText = string.Empty;

    public void Execute()
    {
        _deletedText = editor.Content.Substring(position, length); // capture before deleting
        editor.DeleteAt(position, length);
    }

    public void Undo() => editor.InsertAt(position, _deletedText);
}

// ✅ Invoker — knows nothing about what commands do; only manages the stacks
public sealed class CommandProcessor
{
    private readonly Stack<ICommand> _history   = new();
    private readonly Stack<ICommand> _redoStack = new();

    public void Execute(ICommand command)
    {
        command.Execute();
        _history.Push(command);
        _redoStack.Clear(); // new action invalidates redo stack
    }

    public void Undo()
    {
        if (_history.Count == 0) return;
        var cmd = _history.Pop();
        cmd.Undo();
        _redoStack.Push(cmd);
    }

    public void Redo()
    {
        if (_redoStack.Count == 0) return;
        var cmd = _redoStack.Pop();
        cmd.Execute();
        _history.Push(cmd);
    }

    public bool CanUndo => _history.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
}
