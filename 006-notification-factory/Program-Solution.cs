// =============================================================================
// Hands-On 006 — Program (SOLUTION)
// =============================================================================

using HandsOn.NotificationFactory.PartA;
using HandsOn.NotificationFactory.PartB;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== Part A — Factory Method (fan-out) ===");

var services = new ServiceCollection();
services.AddSingleton<INotificationFactory, EmailNotificationFactory>();
services.AddSingleton<INotificationFactory, SmsNotificationFactory>();
services.AddSingleton<INotificationFactory, PushNotificationFactory>();

var provider = services.BuildServiceProvider();
var factories = provider.GetRequiredService<IEnumerable<INotificationFactory>>();

foreach (var factory in factories)
{
    var notification = factory.Create();
    await notification.SendAsync("alice@example.com", "Server alert: high CPU");
}

Console.WriteLine();
Console.WriteLine("=== Part B — Abstract Factory (family swap) ===");

// ✅ Change only this one line to switch the entire family:
INotificationAbstractFactory family = new EmailNotificationFamily();
// INotificationAbstractFactory family = new SmsNotificationFamily();

var dispatcher = new NotificationDispatcher(family);
await dispatcher.DispatchAsync("alice@example.com", "Alert", "Server CPU > 90%");
