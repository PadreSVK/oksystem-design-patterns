namespace HandsOn021.Infrastructure;

// =============================================================================
// OrderSummaryView — Read Model Entity (shared — always compiled)
// =============================================================================
//
// This is the denormalized read model. The projector creates/updates it
// when domain events are published. The query handler reads from it.
// =============================================================================

public class OrderSummaryView
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public DateTimeOffset PlacedAt { get; set; }
}
