// =============================================================================
// Hands-On 010 — Task 2 Tests: Domain Event Publisher (Observer)
//
// These tests are always compiled and run.
// Fix Task2-Observer.cs in HandsOn010.StrategyObserverCommand/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.StrategyObserverCommand;

namespace HandsOn010.Tests;

public class ObserverTests
{
    [Fact]
    public void Subscribe_SingleObserver_ReceivesPublishedEvent()
    {
        var publisher = new DomainEventPublisher<OrderPlacedEvent>();
        var spy = new SpyObserver<OrderPlacedEvent>();

        publisher.Subscribe(spy);
        var evt = new OrderPlacedEvent(Guid.NewGuid(), 99.99m);
        publisher.Publish(evt);

        Assert.Single(spy.Received);
        Assert.Same(evt, spy.Received[0]);
    }

    [Fact]
    public void Subscribe_MultipleObservers_AllReceiveEvent()
    {
        var publisher = new DomainEventPublisher<OrderPlacedEvent>();
        var spy1 = new SpyObserver<OrderPlacedEvent>();
        var spy2 = new SpyObserver<OrderPlacedEvent>();

        publisher.Subscribe(spy1);
        publisher.Subscribe(spy2);

        publisher.Publish(new OrderPlacedEvent(Guid.NewGuid(), 10m));

        Assert.Single(spy1.Received);
        Assert.Single(spy2.Received);
    }

    [Fact]
    public void Publish_MultipleEvents_AllDeliveredInOrder()
    {
        var publisher = new DomainEventPublisher<OrderPlacedEvent>();
        var spy = new SpyObserver<OrderPlacedEvent>();
        publisher.Subscribe(spy);

        var e1 = new OrderPlacedEvent(Guid.NewGuid(), 1m);
        var e2 = new OrderPlacedEvent(Guid.NewGuid(), 2m);
        var e3 = new OrderPlacedEvent(Guid.NewGuid(), 3m);

        publisher.Publish(e1);
        publisher.Publish(e2);
        publisher.Publish(e3);

        Assert.Equal(3, spy.Received.Count);
        Assert.Same(e1, spy.Received[0]);
        Assert.Same(e2, spy.Received[1]);
        Assert.Same(e3, spy.Received[2]);
    }

    [Fact]
    public void Dispose_Subscription_StopsReceivingEvents()
    {
        var publisher = new DomainEventPublisher<OrderPlacedEvent>();
        var spy = new SpyObserver<OrderPlacedEvent>();

        var subscription = publisher.Subscribe(spy);
        publisher.Publish(new OrderPlacedEvent(Guid.NewGuid(), 5m)); // received

        subscription.Dispose();
        publisher.Publish(new OrderPlacedEvent(Guid.NewGuid(), 5m)); // NOT received

        Assert.Single(spy.Received); // only the first event
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        var publisher = new DomainEventPublisher<OrderPlacedEvent>();
        var spy = new SpyObserver<OrderPlacedEvent>();
        var subscription = publisher.Subscribe(spy);

        subscription.Dispose();
        var ex = Record.Exception(() => subscription.Dispose());

        Assert.Null(ex);
    }

    [Fact]
    public void Subscribe_SameObserverTwice_ReceivesOnlyOnce()
    {
        var publisher = new DomainEventPublisher<OrderPlacedEvent>();
        var spy = new SpyObserver<OrderPlacedEvent>();

        publisher.Subscribe(spy);
        publisher.Subscribe(spy); // duplicate — should be ignored

        publisher.Publish(new OrderPlacedEvent(Guid.NewGuid(), 1m));

        Assert.Single(spy.Received);
    }

    [Fact]
    public void Publisher_WorksWithDifferentEventTypes()
    {
        var publisher = new DomainEventPublisher<OrderShippedEvent>();
        var spy = new SpyObserver<OrderShippedEvent>();
        publisher.Subscribe(spy);

        publisher.Publish(new OrderShippedEvent(Guid.NewGuid(), "TRACK-001"));

        Assert.Single(spy.Received);
        Assert.Equal("TRACK-001", spy.Received[0].TrackingNumber);
    }

    // ── Spy observer ──────────────────────────────────────────────────────────

    private sealed class SpyObserver<T> : IObserver<T>
    {
        public List<T> Received { get; } = [];

        public void OnNext(T value)  => Received.Add(value);
        public void OnError(Exception error)  { }
        public void OnCompleted() { }
    }
}
