namespace HandsOn022.Infrastructure;

// =============================================================================
// IMessagePublisher — shared abstraction for the message broker
// =============================================================================

public interface IMessagePublisher
{
    Task PublishAsync(string type, string payload, CancellationToken ct = default);
}
