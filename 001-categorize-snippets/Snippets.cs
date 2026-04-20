// ============================================================
// Hands-On 001 — Categorize Code Snippets
//
// Instructions:
//   For each snippet below, answer in answers.md:
//   1. What pattern or idiom is being used (or attempted)?
//   2. What category does it belong to?
//      (creational / structural / behavioral / idiom)
//   3. Is there an anti-pattern present? Name it.
//
// Be prepared to explain Snippet 3 — it contains a subtle violation.
// ============================================================

#pragma warning disable CS0618 // intentional for exercise purposes

using System;
using System.Collections.Generic;

namespace HandsOn.CategorizeSnippets;

// ============================================================
// Snippet 1 — What pattern is this? Is it correct?
// ============================================================

public sealed class AuditLogger
{
    private static AuditLogger? _instance;

    private AuditLogger() { }

    // Q: Is this thread-safe? What could go wrong under concurrency?
    public static AuditLogger Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = new AuditLogger();
            }
            return _instance;
        }
    }

    public void Log(string message) =>
        Console.WriteLine($"[AUDIT] {message}");
}

// ============================================================
// Snippet 2 — What pattern is this? What is the problem?
// ============================================================

public interface IOrderEventListener
{
    void OnOrderPlaced(string orderId);
}

public class OrderEventSource
{
    private readonly List<IOrderEventListener> _listeners = new();

    public void Subscribe(IOrderEventListener listener) =>
        _listeners.Add(listener);

    // Q: Is there a way to unsubscribe? What happens over time?
    public void PlaceOrder(string orderId)
    {
        Console.WriteLine($"Order {orderId} placed.");
        foreach (var listener in _listeners)
            listener.OnOrderPlaced(orderId);
    }
}

public class EmailConfirmationService : IOrderEventListener
{
    public void OnOrderPlaced(string orderId) =>
        Console.WriteLine($"[EMAIL] Confirmation sent for order {orderId}");
}

// ============================================================
// Snippet 3 — What is happening here? Is there a violation?
// ============================================================

// A thin wrapper around a third-party reporting library.
// The underlying library does NOT implement IDisposable.
public class ReportContext
{
    private readonly string _reportName;

    public ReportContext(string reportName)
    {
        _reportName = reportName;
        Console.WriteLine($"[ReportContext] Opening context for '{_reportName}'");
    }

    public void Generate() =>
        Console.WriteLine($"[ReportContext] Generating report '{_reportName}'");

    // Q: Does this class need to be in a using block?
    // Q: What expectation does a using block set for the caller?
    // Q: What subtle contract violation is present here?
}

public class ReportRunner
{
    public void Run()
    {
        // Snippet 3 usage — spot the subtle issue
        using (var ctx = (dynamic)new ReportContext("Monthly Sales"))
        {
            ctx.Generate();
        }
        // caller expects Dispose() to be called here — but it won't be
    } 
}
