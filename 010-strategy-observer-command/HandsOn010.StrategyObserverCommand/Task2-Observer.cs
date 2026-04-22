// =============================================================================
// Hands-On 010 — Task 2: Domain Event Publisher with IObservable<T>  (starter)
//
// Scenario: when an order is placed or shipped the domain needs to notify any
//           number of interested parties (email service, audit log, analytics)
//           without the publisher knowing who is listening.
//
// Your task:
//   Implement DomainEventPublisher<T> so that:
//   • Subscribe(observer) — registers the observer and returns an IDisposable
//     token; disposing the token removes the observer from the subscriber list
//   • Publish(event)      — calls OnNext on every registered observer
//
// Observer pattern reminder:
//   • IObservable<T>            = Subject interface (.NET built-in)
//   • IObserver<T>              = Observer interface (.NET built-in)
//   • DomainEventPublisher<T>   = Concrete Subject you implement
//   • Subscription (inner class)= the IDisposable unsubscribe token
// =============================================================================

namespace HandsOn.StrategyObserverCommand;

// ─── Domain events (provided — do not modify) ─────────────────────────────────

public abstract record DomainEvent(Guid EventId, DateTimeOffset OccurredAt);

public sealed record OrderPlacedEvent(Guid OrderId, decimal Total)
    : DomainEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);

public sealed record OrderShippedEvent(Guid OrderId, string TrackingNumber)
    : DomainEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);

// ─── Publisher (your task) ───────────────────────────────────────────────────

public sealed class DomainEventPublisher<T> : IObservable<T>
{
    private readonly List<IObserver<T>> _observers = [];

    /// <summary>
    /// Registers an observer. Returns a token — dispose it to unsubscribe.
    /// </summary>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        // TODO: add observer to _observers (if not already present)
        // TODO: return a Subscription instance that removes this observer on Dispose
        throw new NotImplementedException();
    }

    /// <summary>Delivers the event to all current subscribers.</summary>
    public void Publish(T @event)
    {
        // TODO: call observer.OnNext(@event) for each observer in _observers
        //       iterate over a snapshot copy to handle concurrent unsubscription safely
        throw new NotImplementedException();
    }

    // ── Unsubscribe token ──────────────────────────────────────────────────

    private sealed class Subscription(List<IObserver<T>> observers, IObserver<T> observer)
        : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            // TODO: if not already disposed, remove observer from observers and set _disposed = true
            throw new NotImplementedException();
        }
    }
}
