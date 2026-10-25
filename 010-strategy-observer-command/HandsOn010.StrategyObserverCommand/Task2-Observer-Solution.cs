// =============================================================================
// Hands-On 010 — Task 2: Domain Event Publisher with IObservable<T>  (solution)
// =============================================================================

namespace HandsOn.StrategyObserverCommand;

public abstract record DomainEvent(Guid EventId, DateTimeOffset OccurredAt);

public sealed record OrderPlacedEvent(Guid OrderId, decimal Total)
    : DomainEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);

public sealed record OrderShippedEvent(Guid OrderId, string TrackingNumber)
    : DomainEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);

// ✅ Concrete Subject — any number of observers can subscribe without the
//    publisher knowing who they are or how many there are.
public sealed class DomainEventPublisher<T> : IObservable<T>
{
    private readonly List<IObserver<T>> _observers = [];

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);

        return new Subscription(_observers, observer);
    }

    public void Publish(T @event)
    {
        foreach (var observer in _observers.ToList()) // snapshot — safe during Dispose
            observer.OnNext(@event);
    }

    private sealed class Subscription(List<IObserver<T>> observers, IObserver<T> observer)
        : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            observers.Remove(observer);
            _disposed = true;
        }
    }
}
