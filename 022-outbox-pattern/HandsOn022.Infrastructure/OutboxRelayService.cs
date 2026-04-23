using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HandsOn022.Infrastructure;

// =============================================================================
// Task 2 — OutboxRelayService (Background Relay)
// =============================================================================
//
// The relay is a BackgroundService that polls the outbox table for unsent
// messages and publishes them to the message broker via IMessagePublisher.
//
// YOUR TASKS:
//   1. Extend BackgroundService.
//   2. Accept IServiceScopeFactory and IMessagePublisher via constructor.
//   3. In ExecuteAsync, loop while not cancelled:
//      a. Create a new scope → resolve AppDbContext
//      b. Query unsent OutboxMessages ordered by CreatedAt, take up to 50
//      c. For each message:
//         - Call _publisher.PublishAsync(msg.Type, msg.Payload, ct)
//         - Set msg.Sent = true
//      d. Call db.SaveChangesAsync(ct)
//      e. Wait 5 seconds before next poll (Task.Delay)
//
// KEY INSIGHT: The relay guarantees at-least-once delivery. If the app
//              crashes after writing to the outbox but before publishing,
//              the relay will pick up the message on next start.
//
// IMPORTANT: Consumers must be IDEMPOTENT — the same message may be
//            delivered more than once if a crash occurs after publishing
//            but before marking as sent.
//
// HINT: Use IServiceScopeFactory to create a new DI scope per iteration
//       because BackgroundService is a singleton but DbContext is scoped.
// =============================================================================

public class OutboxRelayService
    // TODO: extend BackgroundService
    : BackgroundService
{
    // TODO: Accept IServiceScopeFactory and IMessagePublisher via constructor

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: Loop while not cancelled:
        //   1. Create scope → resolve AppDbContext
        //   2. Query db.OutboxMessages.Where(m => !m.Sent).OrderBy(m => m.CreatedAt).Take(50)
        //   3. For each: await _publisher.PublishAsync(...); msg.Sent = true;
        //   4. await db.SaveChangesAsync(stoppingToken);
        //   5. await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        throw new NotImplementedException();
    }
}
