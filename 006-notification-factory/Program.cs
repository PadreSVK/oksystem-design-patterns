// =============================================================================
// Hands-On 006 — Program.cs (entry point)
//
// Part A — Factory Method
//   Complete the TODOs in PartA-FactoryMethod.cs, then wire up below.
//
// Part B — Abstract Factory
//   After Part A works, swap the dispatcher to use NotificationDispatcher
//   from PartB-AbstractFactory.cs.  Change only ONE line to switch families.
// =============================================================================

using HandsOn.NotificationFactory.PartA;
using Microsoft.Extensions.DependencyInjection;

// ── Part A: DI wiring ──────────────────────────────────────────────────────
var services = new ServiceCollection();

// TODO: register all three concrete factories as INotificationFactory
//   services.AddSingleton<INotificationFactory, EmailNotificationFactory>();
//   services.AddSingleton<INotificationFactory, SmsNotificationFactory>();
//   services.AddSingleton<INotificationFactory, PushNotificationFactory>();

var provider = services.BuildServiceProvider();

// TODO: resolve IEnumerable<INotificationFactory>, create each notification,
//       and call SendAsync("alice@example.com", "Server alert: high CPU")
//
// Expected output (order may vary):
//   [Email] To: alice@example.com | Server alert: high CPU
//   [SMS]   To: alice@example.com | Server alert: high CPU
//   [Push]  To: alice@example.com | Server alert: high CPU

// ── Part B: Abstract Factory ───────────────────────────────────────────────
// TODO: after completing PartB-AbstractFactory.cs, add:
//
//   using HandsOn.NotificationFactory.PartB;
//   INotificationAbstractFactory family = new EmailNotificationFamily();
//   // ↑ change ONE line to switch to SmsNotificationFamily — nothing else changes
//   var dispatcher = new NotificationDispatcher(family);
//   await dispatcher.DispatchAsync("alice@example.com", "Alert", "Server CPU > 90%");
//
// Expected output with EmailNotificationFamily:
//   [Email] alice@example.com << <h1>Alert</h1><p>Server CPU > 90%</p>
//
// Expected output with SmsNotificationFamily:
//   [SMS]   alice@example.com << Alert\nServer CPU > 90%
