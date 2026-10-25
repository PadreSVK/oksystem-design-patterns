// =============================================================================
// Hands-On 005 — SOLUTION  (NotificationService.cs)
// =============================================================================
using System.Collections.Concurrent;

namespace HandsOn.ToolingSetup;

/// <summary>Sends in-process notifications and records a log of all dispatches.</summary>
public class NotificationService
{
    /// <summary>Gets or sets the default sender address.</summary>
    public string DefaultSender { get; set; } = "noreply@example.com";  // ✅ CA1051 fixed: property

    private readonly ConcurrentBag<string> _log = [];                   // ✅ naming fixed: _log

    /// <summary>Sends a notification to the specified recipient.</summary>
    /// <param name="recipient">The recipient address. Must not be null or empty.</param>
    /// <param name="message">The message body. Must not be null or empty.</param>
    public async Task SendAsync(string recipient, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);           // ✅ CA1062 fixed
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        // ✅ CA1305 fixed: use UtcNow + invariant culture via FormattableString.Invariant
        var entry = FormattableString.Invariant($"{DateTime.UtcNow:O}: {recipient} — {message}");
        _log.Add(entry);

        await Task.Delay(1).ConfigureAwait(false);                      // ✅ CA2007 fixed
    }

    /// <summary>Formats a notification subject with a bracketed prefix.</summary>
    public static string FormatSubject(string prefix, string subject)   // ✅ CA1822 fixed: static
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);              // ✅ CA1062 fixed
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        // ✅ CA1307 fixed: ToUpperInvariant specifies culture explicitly
        return $"[{prefix.ToUpperInvariant()}] {subject}";
    }

    /// <summary>Gets an immutable snapshot of all dispatched notification entries.</summary>
    public IReadOnlyCollection<string> Log => _log;                     // ✅ CA1024 fixed: property
}
