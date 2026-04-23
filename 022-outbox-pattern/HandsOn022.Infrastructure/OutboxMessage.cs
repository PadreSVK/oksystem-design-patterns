namespace HandsOn022.Infrastructure;

// =============================================================================
// OutboxMessage Entity (shared — always compiled)
// =============================================================================

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = "";
    public string Payload { get; set; } = "";
    public bool Sent { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
