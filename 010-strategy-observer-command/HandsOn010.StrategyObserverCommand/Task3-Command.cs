// =============================================================================
// Hands-On 010 — Task 3: Undo/Redo Text Editor with Command  (starter)
//
// Scenario: a simple text editor must support unlimited undo and redo.
//           Each editing operation is encapsulated as a Command object so the
//           processor can replay or reverse any action without knowing the
//           details of what changed.
//
// Your tasks:
//
//   Task 3a — InsertTextCommand:
//     • Execute: insert Text at Position in the editor
//     • Undo:    delete the same Text starting at Position
//
//   Task 3b — DeleteTextCommand:
//     • Execute: capture the text being deleted, then delete Length chars at Position
//     • Undo:    re-insert the captured text at Position
//
//   Task 3c — CommandProcessor:
//     • Execute(cmd): run cmd, push to history, clear redo stack
//     • Undo():       pop from history, call Undo(), push to redo stack
//     • Redo():       pop from redo stack, call Execute(), push back to history
//
// Command pattern reminder:
//   • ICommand          = Command interface
//   • InsertTextCommand / DeleteTextCommand = Concrete Commands
//   • TextEditor        = Receiver (provided — do not modify)
//   • CommandProcessor  = Invoker — manages history and delegates to commands
// =============================================================================

namespace HandsOn.StrategyObserverCommand;

// ─── Command interface ────────────────────────────────────────────────────────

public interface ICommand
{
    void Execute();
    void Undo();
}

// ─── Receiver (provided — do not modify) ─────────────────────────────────────

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

// ─── Concrete commands (your task) ───────────────────────────────────────────

public sealed class InsertTextCommand(TextEditor editor, int position, string text) : ICommand
{
    private readonly TextEditor _editor = editor;
    private readonly int _position = position;
    private readonly string _text = text;

    public void Execute()
    {
        // TODO: _editor.InsertAt(_position, _text)
        throw new NotImplementedException();
    }

    public void Undo()
    {
        // TODO: _editor.DeleteAt(_position, _text.Length)
        throw new NotImplementedException();
    }
}

public sealed class DeleteTextCommand(TextEditor editor, int position, int length) : ICommand
{
    private readonly TextEditor _editor = editor;
    private readonly int _position = position;
    private readonly int _length = length;
    private string _deletedText = string.Empty; // captured on Execute for Undo

    public void Execute()
    {
        // TODO: _deletedText = _editor.Content.Substring(_position, _length)
        // TODO: _editor.DeleteAt(_position, _length)
        throw new NotImplementedException();
    }

    public void Undo()
    {
        // TODO: _editor.InsertAt(_position, _deletedText)
        throw new NotImplementedException();
    }
}

// ─── Invoker (your task) ─────────────────────────────────────────────────────

public sealed class CommandProcessor
{
    private readonly Stack<ICommand> _history = new();
    private readonly Stack<ICommand> _redoStack = new();

    /// <summary>Executes the command and records it for future undo.</summary>
    public void Execute(ICommand command)
    {
        // TODO: command.Execute()
        // TODO: _history.Push(command)
        // TODO: _redoStack.Clear()   ← new action invalidates the redo stack
        throw new NotImplementedException();
    }

    /// <summary>Undoes the most recent command. No-op if history is empty.</summary>
    public void Undo()
    {
        // TODO: if (_history.Count == 0) return
        // TODO: var cmd = _history.Pop(); cmd.Undo(); _redoStack.Push(cmd)
        throw new NotImplementedException();
    }

    /// <summary>Re-applies the most recently undone command. No-op if redo stack is empty.</summary>
    public void Redo()
    {
        // TODO: if (_redoStack.Count == 0) return
        // TODO: var cmd = _redoStack.Pop(); cmd.Execute(); _history.Push(cmd)
        throw new NotImplementedException();
    }

    public bool CanUndo => _history.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
}
