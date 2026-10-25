using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HandsOn022.Infrastructure;

// =============================================================================
// Task 2 Solution — OutboxRelayService (Background Relay)
// =============================================================================

public class OutboxRelayService(
    IServiceScopeFactory scopeFactory,
    IMessagePublisher publisher) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pending = await db.OutboxMessages
                .Where(m => !m.Sent)
                .OrderBy(m => m.CreatedAt)
                .Take(50)
                .ToListAsync(stoppingToken);

            foreach (var msg in pending)
            {
                await publisher.PublishAsync(msg.Type, msg.Payload, stoppingToken);
                msg.Sent = true;
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
