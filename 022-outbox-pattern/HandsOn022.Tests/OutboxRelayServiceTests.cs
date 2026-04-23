using HandsOn022.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HandsOn022.Tests;

public class OutboxRelayServiceTests
{
    private class SpyMessagePublisher : IMessagePublisher
    {
        public List<(string Type, string Payload)> Published { get; } = [];

        public Task PublishAsync(string type, string payload, CancellationToken ct = default)
        {
            Published.Add((type, payload));
            return Task.CompletedTask;
        }
    }

    private static ServiceProvider BuildServiceProvider(string dbName)
    {
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseInMemoryDatabase(dbName));
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task ExecuteAsync_publishes_unsent_messages()
    {
        var dbName = Guid.NewGuid().ToString();
        var sp = BuildServiceProvider(dbName);
        var publisher = new SpyMessagePublisher();

        // Seed an unsent outbox message
        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.OutboxMessages.Add(new OutboxMessage
            {
                Type = "OrderPlaced",
                Payload = """{"OrderId":"abc"}""",
                Sent = false
            });
            await db.SaveChangesAsync();
        }

        // Run one iteration of the relay
        var relay = new OutboxRelayService(sp.GetRequiredService<IServiceScopeFactory>(), publisher);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(8));

        try { await relay.StartAsync(cts.Token); await Task.Delay(1500, cts.Token); }
        catch (OperationCanceledException) { }
        finally { await relay.StopAsync(CancellationToken.None); }

        Assert.Single(publisher.Published);
        Assert.Equal("OrderPlaced", publisher.Published[0].Type);
    }

    [Fact]
    public async Task ExecuteAsync_marks_messages_as_sent()
    {
        var dbName = Guid.NewGuid().ToString();
        var sp = BuildServiceProvider(dbName);
        var publisher = new SpyMessagePublisher();

        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.OutboxMessages.Add(new OutboxMessage
            {
                Type = "OrderPlaced",
                Payload = """{"OrderId":"abc"}""",
                Sent = false
            });
            await db.SaveChangesAsync();
        }

        var relay = new OutboxRelayService(sp.GetRequiredService<IServiceScopeFactory>(), publisher);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(8));

        try { await relay.StartAsync(cts.Token); await Task.Delay(1500, cts.Token); }
        catch (OperationCanceledException) { }
        finally { await relay.StopAsync(CancellationToken.None); }

        using var scope2 = sp.CreateScope();
        var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
        var msg = await db2.OutboxMessages.SingleAsync();
        Assert.True(msg.Sent);
    }

    [Fact]
    public async Task ExecuteAsync_skips_already_sent_messages()
    {
        var dbName = Guid.NewGuid().ToString();
        var sp = BuildServiceProvider(dbName);
        var publisher = new SpyMessagePublisher();

        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.OutboxMessages.Add(new OutboxMessage
            {
                Type = "OrderPlaced",
                Payload = """{"OrderId":"old"}""",
                Sent = true
            });
            db.OutboxMessages.Add(new OutboxMessage
            {
                Type = "OrderShipped",
                Payload = """{"OrderId":"new"}""",
                Sent = false
            });
            await db.SaveChangesAsync();
        }

        var relay = new OutboxRelayService(sp.GetRequiredService<IServiceScopeFactory>(), publisher);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(8));

        try { await relay.StartAsync(cts.Token); await Task.Delay(1500, cts.Token); }
        catch (OperationCanceledException) { }
        finally { await relay.StopAsync(CancellationToken.None); }

        Assert.Single(publisher.Published);
        Assert.Equal("OrderShipped", publisher.Published[0].Type);
    }

    [Fact]
    public async Task ExecuteAsync_handles_empty_outbox()
    {
        var dbName = Guid.NewGuid().ToString();
        var sp = BuildServiceProvider(dbName);
        var publisher = new SpyMessagePublisher();

        var relay = new OutboxRelayService(sp.GetRequiredService<IServiceScopeFactory>(), publisher);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(8));

        try { await relay.StartAsync(cts.Token); await Task.Delay(1500, cts.Token); }
        catch (OperationCanceledException) { }
        finally { await relay.StopAsync(CancellationToken.None); }

        Assert.Empty(publisher.Published);
    }
}
