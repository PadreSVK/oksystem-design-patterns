// ============================================================
// Hands-On 001 — Categorize Code Snippets — SOLUTION
// ============================================================

using System;
using System.Collections.Generic;

namespace HandsOn.CategorizeSnippets.Solution;

// ============================================================
// Snippet 1 — AuditLogger
//
// PATTERN   : Singleton (Creational)
// CATEGORY  : Creational
// ANTI-PATTERN: Non-thread-safe Singleton
//   _instance is not volatile and there is no lock.
//   Two threads can both pass the `is null` check simultaneously
//   and create two instances, breaking the "exactly one" guarantee.
//
// FIX: Use Lazy<T> — lazy, thread-safe, no manual locking needed.
// ============================================================

// ❌ Original — broken under concurrency
public sealed class AuditLogger_Broken
{
    private static AuditLogger_Broken? _instance;

    private AuditLogger_Broken() { }

    public static AuditLogger_Broken Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = new AuditLogger_Broken(); // ❌ race condition
            }
            return _instance;
        }
    }

    public void Log(string message) =>
        Console.WriteLine($"[AUDIT] {message}");
}

// ✅ Fixed — Lazy<T> guarantees single initialisation and thread safety
public sealed class AuditLogger
{
    private static readonly Lazy<AuditLogger> _lazy =
        new(() => new AuditLogger(), isThreadSafe: true);

    private AuditLogger() { }

    public static AuditLogger Instance => _lazy.Value;

    public void Log(string message) =>
        Console.WriteLine($"[AUDIT] {message}");
}

// ============================================================
// Snippet 2 — OrderEventSource / IOrderEventListener
//
// PATTERN   : Observer (Behavioral)
// CATEGORY  : Behavioral
// ANTI-PATTERN: Lapsed Listener / Memory Leak
//   Subscribe() adds listeners but there is no Unsubscribe() or
//   IDisposable return. Listeners are held forever, preventing GC.
//   In long-lived subjects this leaks memory and causes stale
//   objects to receive notifications.
//
// FIX: Return an IDisposable subscription token from Subscribe()
//      so callers can unsubscribe deterministically.
// ============================================================

public interface IOrderEventListener
{
    void OnOrderPlaced(string orderId);
}

// ❌ Original — no unsubscribe path
public class OrderEventSource_Broken
{
    private readonly List<IOrderEventListener> _listeners = new();

    public void Subscribe(IOrderEventListener listener) =>
        _listeners.Add(listener); // ❌ no way to remove

    public void PlaceOrder(string orderId)
    {
        Console.WriteLine($"Order {orderId} placed.");
        foreach (var listener in _listeners)
            listener.OnOrderPlaced(orderId);
    }
}

// ✅ Fixed — subscription token returned; caller disposes to unsubscribe
public class OrderEventSource
{
    private readonly List<IOrderEventListener> _listeners = new();

    public IDisposable Subscribe(IOrderEventListener listener)
    {
        _listeners.Add(listener);
        return new Subscription(() => _listeners.Remove(listener));
    }

    public void PlaceOrder(string orderId)
    {
        Console.WriteLine($"Order {orderId} placed.");
        foreach (var listener in new List<IOrderEventListener>(_listeners))
            listener.OnOrderPlaced(orderId);
    }

    private sealed class Subscription(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}

public class EmailConfirmationService : IOrderEventListener
{
    public void OnOrderPlaced(string orderId) =>
        Console.WriteLine($"[EMAIL] Confirmation sent for order {orderId}");
}

// ============================================================
// Snippet 3 — ReportContext in a using block
//
// PATTERN   : using + IDisposable idiom (attempted)
// CATEGORY  : Idiom
// ANTI-PATTERN: using on a non-IDisposable type via dynamic
//   ReportContext does not implement IDisposable.
//   Casting to dynamic silences the compiler, but at runtime
//   the using block emits a Dispose() call that does not exist,
//   throwing RuntimeBinderException at the closing brace.
//   The class signals deterministic cleanup without honouring
//   the contract.
//
// FIX: Implement IDisposable properly on ReportContext, or
//      remove the using block if no cleanup is needed.
// ============================================================

// ❌ Original — does not implement IDisposable; dynamic cast hides the error
public class ReportContext_Broken
{
    private readonly string _reportName;

    public ReportContext_Broken(string reportName)
    {
        _reportName = reportName;
        Console.WriteLine($"[ReportContext] Opening context for '{_reportName}'");
    }

    public void Generate() =>
        Console.WriteLine($"[ReportContext] Generating report '{_reportName}'");
}

public class ReportRunner_Broken
{
    public void Run()
    {
        // ❌ RuntimeBinderException at runtime — Dispose() does not exist
        using (var ctx = (dynamic)new ReportContext_Broken("Monthly Sales"))
        {
            ctx.Generate();
        }
    }
}

// ✅ Fixed — implements IDisposable; using block works correctly
public class ReportContext : IDisposable
{
    private readonly string _reportName;
    private bool _disposed;

    public ReportContext(string reportName)
    {
        _reportName = reportName;
        Console.WriteLine($"[ReportContext] Opening context for '{_reportName}'");
    }

    public void Generate()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Console.WriteLine($"[ReportContext] Generating report '{_reportName}'");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Console.WriteLine($"[ReportContext] Closing context for '{_reportName}'");
    }
}

public class ReportRunner
{
    public void Run()
    {
        using var ctx = new ReportContext("Monthly Sales"); // ✅ Dispose() guaranteed
        ctx.Generate();
    }
}
 