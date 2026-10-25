// =============================================================================
// Hands-On 013 — GRASP Audit: Refactor a Fat Controller  (solution)
// =============================================================================

namespace HandsOn013.GraspAudit;

// ─── Fat controller (for audit — do not modify) ──────────────────────────────

public class FatOrderController
{
    public void Checkout(Order order)
    {
        if (!ValidateStock(order))
            throw new InvalidOperationException("Out of stock");

        decimal total = CalculateTotal(order);
        order.TotalAmount = total;
        SaveToDb(order);
        SendConfirmation(order);
    }

    private bool ValidateStock(Order order)
    {
        foreach (var line in order.Lines)
        {
            if (line.Quantity > 10) return false;
        }
        return true;
    }

    private decimal CalculateTotal(Order order)
    {
        decimal total = 0;
        foreach (var line in order.Lines)
            total += line.Price * line.Quantity;
        return total;
    }

    private void SaveToDb(Order order)
        => Console.WriteLine($"Saving order {order.Id} to database using SQL...");

    private void SendConfirmation(Order order)
        => Console.WriteLine($"Sending email to {order.CustomerEmail} for order {order.Id}...");
}

// ─── Domain ──────────────────────────────────────────────────────────────────

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public List<OrderLine> Lines { get; set; } = [];
    public decimal TotalAmount { get; set; }

    // ✅ GRASP: Information Expert — Order owns its lines, so it computes the total
    public void CalculateTotal()
    {
        TotalAmount = Lines.Sum(l => l.Price * l.Quantity);
    }
}

public class OrderLine
{
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

// ─── Interfaces — Pure Fabrications ──────────────────────────────────────────

public interface IOrderRepository
{
    void Save(Order order);
}

public interface IEmailService
{
    void SendConfirmation(Order order);
}

public interface IInventoryService
{
    bool IsStockAvailable(Order order);
}

// ─── Refactored controller ───────────────────────────────────────────────────

// ✅ GRASP: Controller — thin, delegates everything
public class OrderController(
    IOrderRepository repository,
    IEmailService emailService,
    IInventoryService inventoryService)
{
    private readonly IOrderRepository _repository = repository;
    private readonly IEmailService _emailService = emailService;
    private readonly IInventoryService _inventoryService = inventoryService;

    public void Checkout(Order order)
    {
        // ✅ GRASP: Information Expert — inventory handles stock validation
        if (!_inventoryService.IsStockAvailable(order))
            throw new InvalidOperationException("Out of stock");

        // ✅ GRASP: Information Expert — Order calculates its own total
        order.CalculateTotal();

        // ✅ GRASP: Pure Fabrication — repository handles persistence
        _repository.Save(order);

        // ✅ GRASP: High Cohesion — EmailService handles notifications
        _emailService.SendConfirmation(order);
    }
}
