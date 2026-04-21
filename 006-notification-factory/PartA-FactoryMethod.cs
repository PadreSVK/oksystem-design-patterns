// =============================================================================
// Hands-On 006 — Part A: Factory Method  (starter)
//
// Complete every TODO block.  Do NOT edit the -Solution.cs files.
// =============================================================================

namespace HandsOn.NotificationFactory.PartA;

// ── Product interface ────────────────────────────────────────────────────────

/// <summary>Represents a single notification that can be sent to a recipient.</summary>
public interface INotification
{
    // TODO: declare  Task SendAsync(string to, string message)
}

// ── Factory interface ────────────────────────────────────────────────────────

/// <summary>Creates an <see cref="INotification"/> instance.</summary>
public interface INotificationFactory
{
    // TODO: declare  INotification Create()
}

// ── Concrete products ────────────────────────────────────────────────────────

// TODO: implement EmailNotification : INotification
//       SendAsync should write:  [Email] To: {to} | {message}

// TODO: implement SmsNotification : INotification
//       SendAsync should write:  [SMS]   To: {to} | {message}

// TODO: implement PushNotification : INotification
//       SendAsync should write:  [Push]  To: {to} | {message}

// ── Concrete factories ───────────────────────────────────────────────────────

// TODO: implement EmailNotificationFactory : INotificationFactory
//       Create() returns new EmailNotification()

// TODO: implement SmsNotificationFactory : INotificationFactory
//       Create() returns new SmsNotification()

// TODO: implement PushNotificationFactory : INotificationFactory
//       Create() returns new PushNotification()
