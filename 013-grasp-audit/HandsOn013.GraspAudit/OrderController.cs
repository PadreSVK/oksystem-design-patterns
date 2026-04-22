// =============================================================================
// Hands-On 013 — GRASP Audit: Refactor a Fat Controller  (starter)
//
// Scenario: OrderController is a "God Class" — it validates stock, calculates
//           totals, saves to a database, and sends confirmation emails.
//           Every one of those responsibilities violates a GRASP principle.
//
// Part A — Audit (read-only):
//   Read FatOrderController below and annotate each method with the GRASP
//   principle it violates (see comments already in the code).
//
// Part B — Refactor (your task):
//   The interfaces IOrderRepository, IEmailService, IInventoryService are
//   already defined at the bottom.  Your tasks:
//
//   Task 1 — Order.CalculateTotal():
//     Information Expert: Order has the line items, so it should compute the total.
//     Implement CalculateTotal() so it sets TotalAmount = sum of Price*Quantity.
//
//   Task 2 — OrderController (refactored):
//     Controller / Low Coupling / High Cohesion / Pure Fabrication:
//     Rewrite OrderController so it accepts the three interfaces via constructor
//     and delegates to them.  The Checkout method should:
//       1. Call _inventoryService.IsStockAvailable(order) — throw if false
//       2. Call order.CalculateTotal()
//       3. Call _repository.Save(order)
//       4. Call _emailService.SendConfirmation(order)
//
// The fat version is kept as FatOrderController for reference.
// =============================================================================

namespace HandsOn013.GraspAudit;

// ─── Fat controller (for audit — do not modify) ──────────────────────────────

/// <summary>
/// The original "God Class" controller that violates multiple GRASP principles.
/// Students: read this, identify violations, then implement the refactored version below.
/// </summary>
public class FatOrderController
{
    // ❌ Violation: High Cohesion — controller handles email and DB details
    // ❌ Violation: Pure Fabrication — controller should delegate persistence

    public void Checkout(Order order)
    {
        // ❌ Violation: Information Expert — logic belongs in Inventory/Product
        if (!ValidateStock(order))
            throw new InvalidOperationException("Out of stock");

        // ❌ Violation: Information Expert — logic belongs in Order
        decimal total = CalculateTotal(order);
        order.TotalAmount = total;

        // ❌ Violation: Pure Fabrication — logic belongs in a Repository
        SaveToDb(order);

        // ❌ Violation: High Cohesion — logic belongs in EmailService
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
        {
            total += line.Price * line.Quantity;
        }
        return total;
    }

    private void SaveToDb(Order order)
        => Console.WriteLine($"Saving order {order.Id} to database using SQL...");

    private void SendConfirmation(Order order)
        => Console.WriteLine($"Sending email to {order.CustomerEmail} for order {order.Id}...");
}

// ─── Domain (shared by fat and refactored versions) ──────────────────────────

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public List<OrderLine> Lines { get; set; } = [];
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// GRASP: Information Expert — Order owns its lines, so it computes the total.
    /// </summary>
    public void CalculateTotal()
    {
        // TODO: TotalAmount = Lines.Sum(l => l.Price * l.Quantity)
        throw new NotImplementedException();
    }
}

public class OrderLine
{
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

// ─── Interfaces — Pure Fabrications (provided — do not modify) ───────────────

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

// ─── Refactored controller (your task — Task 2) ─────────────────────────────

public class OrderController
{
    // TODO: accept IOrderRepository, IEmailService, IInventoryService via constructor
    // TODO: store them in private readonly fields

    public OrderController(
        IOrderRepository repository,
        IEmailService emailService,
        IInventoryService inventoryService)
    {
        throw new NotImplementedException();
    }

    public void Checkout(Order order)
    {
        // TODO: 1. if (!_inventoryService.IsStockAvailable(order)) throw
        // TODO: 2. order.CalculateTotal()
        // TODO: 3. _repository.Save(order)
        // TODO: 4. _emailService.SendConfirmation(order)
        throw new NotImplementedException();
    }
}
