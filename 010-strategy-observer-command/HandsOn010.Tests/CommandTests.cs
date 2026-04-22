// =============================================================================
// Hands-On 010 — Task 3 Tests: Undo/Redo Text Editor (Command)
//
// These tests are always compiled and run.
// Fix Task3-Command.cs in HandsOn010.StrategyObserverCommand/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.StrategyObserverCommand;

namespace HandsOn010.Tests;

public class CommandTests
{
    // ── InsertTextCommand ─────────────────────────────────────────────────────

    [Fact]
    public void InsertTextCommand_Execute_InsertsTextAtPosition()
    {
        var editor  = new TextEditor();
        var command = new InsertTextCommand(editor, 0, "Hello");

        command.Execute();

        Assert.Equal("Hello", editor.Content);
    }

    [Fact]
    public void InsertTextCommand_Undo_RemovesInsertedText()
    {
        var editor  = new TextEditor();
        var command = new InsertTextCommand(editor, 0, "Hello");

        command.Execute();
        command.Undo();

        Assert.Equal(string.Empty, editor.Content);
    }

    [Fact]
    public void InsertTextCommand_Execute_AtMiddlePosition()
    {
        var editor = new TextEditor();
        new InsertTextCommand(editor, 0, "Helo").Execute();
        new InsertTextCommand(editor, 3, "l").Execute(); // "Helo" → "Hello"

        Assert.Equal("Hello", editor.Content);
    }

    // ── DeleteTextCommand ─────────────────────────────────────────────────────

    [Fact]
    public void DeleteTextCommand_Execute_RemovesCharacters()
    {
        var editor = new TextEditor();
        new InsertTextCommand(editor, 0, "Hello World").Execute();

        var command = new DeleteTextCommand(editor, 5, 6); // delete " World"
        command.Execute();

        Assert.Equal("Hello", editor.Content);
    }

    [Fact]
    public void DeleteTextCommand_Undo_RestoresDeletedText()
    {
        var editor = new TextEditor();
        new InsertTextCommand(editor, 0, "Hello World").Execute();

        var command = new DeleteTextCommand(editor, 5, 6);
        command.Execute();
        command.Undo();

        Assert.Equal("Hello World", editor.Content);
    }

    // ── CommandProcessor ──────────────────────────────────────────────────────

    [Fact]
    public void CommandProcessor_Execute_AppliesCommand()
    {
        var editor    = new TextEditor();
        var processor = new CommandProcessor();

        processor.Execute(new InsertTextCommand(editor, 0, "Hi"));

        Assert.Equal("Hi", editor.Content);
    }

    [Fact]
    public void CommandProcessor_Undo_RevertsLastCommand()
    {
        var editor    = new TextEditor();
        var processor = new CommandProcessor();

        processor.Execute(new InsertTextCommand(editor, 0, "Hello"));
        processor.Undo();

        Assert.Equal(string.Empty, editor.Content);
    }

    [Fact]
    public void CommandProcessor_Undo_MultipleCommands_InReverseOrder()
    {
        var editor    = new TextEditor();
        var processor = new CommandProcessor();

        processor.Execute(new InsertTextCommand(editor, 0, "Hello"));
        processor.Execute(new InsertTextCommand(editor, 5, " World"));
        Assert.Equal("Hello World", editor.Content);

        processor.Undo(); // removes " World"
        Assert.Equal("Hello", editor.Content);

        processor.Undo(); // removes "Hello"
        Assert.Equal(string.Empty, editor.Content);
    }

    [Fact]
    public void CommandProcessor_Redo_ReappliesUndoneCommand()
    {
        var editor    = new TextEditor();
        var processor = new CommandProcessor();

        processor.Execute(new InsertTextCommand(editor, 0, "Hello"));
        processor.Undo();
        processor.Redo();

        Assert.Equal("Hello", editor.Content);
    }

    [Fact]
    public void CommandProcessor_NewExecute_ClearsRedoStack()
    {
        var editor    = new TextEditor();
        var processor = new CommandProcessor();

        processor.Execute(new InsertTextCommand(editor, 0, "Hello"));
        processor.Undo();

        Assert.True(processor.CanRedo);

        processor.Execute(new InsertTextCommand(editor, 0, "World")); // clears redo

        Assert.False(processor.CanRedo);
    }

    [Fact]
    public void CommandProcessor_Undo_EmptyHistory_DoesNotThrow()
    {
        var processor = new CommandProcessor();
        var ex = Record.Exception(() => processor.Undo());
        Assert.Null(ex);
    }

    [Fact]
    public void CommandProcessor_Redo_EmptyStack_DoesNotThrow()
    {
        var processor = new CommandProcessor();
        var ex = Record.Exception(() => processor.Redo());
        Assert.Null(ex);
    }

    [Fact]
    public void CommandProcessor_CanUndo_CanRedo_ReflectState()
    {
        var editor    = new TextEditor();
        var processor = new CommandProcessor();

        Assert.False(processor.CanUndo);
        Assert.False(processor.CanRedo);

        processor.Execute(new InsertTextCommand(editor, 0, "X"));
        Assert.True(processor.CanUndo);
        Assert.False(processor.CanRedo);

        processor.Undo();
        Assert.False(processor.CanUndo);
        Assert.True(processor.CanRedo);

        processor.Redo();
        Assert.True(processor.CanUndo);
        Assert.False(processor.CanRedo);
    }
}
