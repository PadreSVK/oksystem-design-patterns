// =============================================================================
// Hands-On 006 — Part B: Abstract Factory  (starter)
//
// Building on Part A: pair each transport with a matching message formatter
// so the whole "family" is swapped via a single DI registration change.
//
// Complete every TODO block.  Do NOT edit the -Solution.cs files.
// =============================================================================

namespace HandsOn.NotificationFactory.PartB;

// ── Product interfaces ───────────────────────────────────────────────────────

/// <summary>Formats a subject + body into a transport-appropriate payload string.</summary>
public interface IMessageFormatter
{
    // TODO: declare  string Format(string subject, string body)
}

/// <summary>Sends a pre-formatted payload to a recipient.</summary>
public interface INotificationSender
{
    // TODO: declare  Task SendAsync(string recipient, string payload)
}

// ── Abstract factory interface ───────────────────────────────────────────────

/// <summary>Creates a matched pair of sender + formatter for one notification family.</summary>
public interface INotificationAbstractFactory
{
    // TODO: declare  INotificationSender  CreateSender()
    // TODO: declare  IMessageFormatter    CreateFormatter()
}

// ── Formatters ───────────────────────────────────────────────────────────────

// TODO: HtmlFormatter : IMessageFormatter
//       Format returns:  $"<h1>{subject}</h1><p>{body}</p>"

// TODO: PlainTextFormatter : IMessageFormatter
//       Format returns:  $"{subject}\n{body}"

// TODO: JsonPayloadFormatter : IMessageFormatter
//       Format returns:  $"{{\"subject\":\"{subject}\",\"body\":\"{body}\"}}"

// ── Senders ──────────────────────────────────────────────────────────────────

// TODO: EmailSender : INotificationSender
//       SendAsync writes:  [Email] {recipient} << {payload}

// TODO: SmsSender : INotificationSender
//       SendAsync writes:  [SMS]   {recipient} << {payload}

// TODO: PushSender : INotificationSender
//       SendAsync writes:  [Push]  {recipient} << {payload}

// ── Concrete families ────────────────────────────────────────────────────────

// TODO: EmailNotificationFamily : INotificationAbstractFactory
//       CreateSender()    => new EmailSender()
//       CreateFormatter() => new HtmlFormatter()

// TODO: SmsNotificationFamily : INotificationAbstractFactory
//       CreateSender()    => new SmsSender()
//       CreateFormatter() => new PlainTextFormatter()

// ── Stretch goal: Null Object family (for tests) ─────────────────────────────
// TODO: NullNotificationFamily : INotificationAbstractFactory
//       CreateSender()    => NullSender (records calls, never writes to console/network)
//       CreateFormatter() => NullFormatter (returns subject + body as-is)
//       Expose SentMessages: IReadOnlyList<(string Recipient, string Payload)> for assertions

// ── Client / dispatcher ──────────────────────────────────────────────────────

// TODO: NotificationDispatcher that receives INotificationAbstractFactory via constructor
//       DispatchAsync(string to, string subject, string body) should:
//         1. Call factory.CreateFormatter().Format(subject, body)
//         2. Call factory.CreateSender().SendAsync(to, formattedPayload)
