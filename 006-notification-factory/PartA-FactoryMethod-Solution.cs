// =============================================================================
// Hands-On 006 — Part A: Factory Method  (SOLUTION)
// =============================================================================

namespace HandsOn.NotificationFactory.PartA;

// ── Product interface ────────────────────────────────────────────────────────

public interface INotification
{
    Task SendAsync(string to, string message);
}

// ── Factory interface ────────────────────────────────────────────────────────

public interface INotificationFactory
{
    INotification Create();
}

// ── Concrete products ────────────────────────────────────────────────────────

public sealed class EmailNotification : INotification
{
    public Task SendAsync(string to, string message)
    {
        Console.WriteLine($"[Email] To: {to} | {message}");
        return Task.CompletedTask;
    }
}

public sealed class SmsNotification : INotification
{
    public Task SendAsync(string to, string message)
    {
        Console.WriteLine($"[SMS]   To: {to} | {message}");
        return Task.CompletedTask;
    }
}

public sealed class PushNotification : INotification
{
    public Task SendAsync(string to, string message)
    {
        Console.WriteLine($"[Push]  To: {to} | {message}");
        return Task.CompletedTask;
    }
}

// ── Concrete factories ───────────────────────────────────────────────────────

public sealed class EmailNotificationFactory : INotificationFactory
{
    public INotification Create() => new EmailNotification();
}

public sealed class SmsNotificationFactory : INotificationFactory
{
    public INotification Create() => new SmsNotification();
}

public sealed class PushNotificationFactory : INotificationFactory
{
    public INotification Create() => new PushNotification();
}
