// =============================================================================
// Hands-On 006 — Part B: Abstract Factory  (SOLUTION)
// =============================================================================

namespace HandsOn.NotificationFactory.PartB;

// ── Product interfaces ───────────────────────────────────────────────────────

public interface IMessageFormatter
{
    string Format(string subject, string body);
}

public interface INotificationSender
{
    Task SendAsync(string recipient, string payload);
}

// ── Abstract factory interface ───────────────────────────────────────────────

public interface INotificationAbstractFactory
{
    INotificationSender CreateSender();
    IMessageFormatter   CreateFormatter();
}

// ── Formatters ───────────────────────────────────────────────────────────────

public sealed class HtmlFormatter : IMessageFormatter
{
    public string Format(string subject, string body) =>
        $"<h1>{subject}</h1><p>{body}</p>";
}

public sealed class PlainTextFormatter : IMessageFormatter
{
    public string Format(string subject, string body) =>
        $"{subject}\n{body}";
}

public sealed class JsonPayloadFormatter : IMessageFormatter
{
    public string Format(string subject, string body) =>
        $"{{\"subject\":\"{subject}\",\"body\":\"{body}\"}}";
}

// ── Senders ──────────────────────────────────────────────────────────────────

public sealed class EmailSender : INotificationSender
{
    public Task SendAsync(string recipient, string payload)
    {
        Console.WriteLine($"[Email] {recipient} << {payload}");
        return Task.CompletedTask;
    }
}

public sealed class SmsSender : INotificationSender
{
    public Task SendAsync(string recipient, string payload)
    {
        Console.WriteLine($"[SMS]   {recipient} << {payload}");
        return Task.CompletedTask;
    }
}

public sealed class PushSender : INotificationSender
{
    public Task SendAsync(string recipient, string payload)
    {
        Console.WriteLine($"[Push]  {recipient} << {payload}");
        return Task.CompletedTask;
    }
}

// ── Concrete families ────────────────────────────────────────────────────────

public sealed class EmailNotificationFamily : INotificationAbstractFactory
{
    public INotificationSender CreateSender()    => new EmailSender();
    public IMessageFormatter   CreateFormatter() => new HtmlFormatter();
}

public sealed class SmsNotificationFamily : INotificationAbstractFactory
{
    public INotificationSender CreateSender()    => new SmsSender();
    public IMessageFormatter   CreateFormatter() => new PlainTextFormatter();
}

/// <summary>
/// Null Object family — silently discards all sends; records calls for test assertions.
/// Useful as a test double: swap the real family for this one in unit tests without
/// touching <see cref="NotificationDispatcher"/> at all.
/// </summary>
public sealed class NullNotificationFamily : INotificationAbstractFactory
{
    private readonly NullSender    _sender    = new();
    private readonly NullFormatter _formatter = new();

    public INotificationSender CreateSender()    => _sender;
    public IMessageFormatter   CreateFormatter() => _formatter;

    /// <summary>All (recipient, payload) pairs recorded by <see cref="NullSender.SendAsync"/>.</summary>
    public IReadOnlyList<(string Recipient, string Payload)> SentMessages => _sender.SentMessages;
}

internal sealed class NullFormatter : IMessageFormatter
{
    // Returns the raw subject + body unchanged — no markup, no side effects.
    public string Format(string subject, string body) => $"{subject}: {body}";
}

internal sealed class NullSender : INotificationSender
{
    private readonly List<(string Recipient, string Payload)> _sent = [];

    public IReadOnlyList<(string Recipient, string Payload)> SentMessages => _sent;

    public Task SendAsync(string recipient, string payload)
    {
        _sent.Add((recipient, payload));   // record — do NOT write to console or network
        return Task.CompletedTask;
    }
}

// ── Client / dispatcher ──────────────────────────────────────────────────────

public sealed class NotificationDispatcher(INotificationAbstractFactory factory)
{
    public async Task DispatchAsync(string to, string subject, string body)
    {
        var formatter = factory.CreateFormatter();
        var sender    = factory.CreateSender();
        await sender.SendAsync(to, formatter.Format(subject, body));
    }
}
