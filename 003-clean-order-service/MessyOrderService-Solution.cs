// =============================================================================
// Hands-On 003 — Clean Up OrderService  ★ SOLUTION ★
// =============================================================================
//
// VIOLATIONS FOUND IN MessyOrderService.cs (Step 1 answer):
//
//   ❌  ord          → Order
//   ❌  itm          → LineItem
//   ❌  OrdSvc       → OrderService
//   ❌  ordId        → Id
//   ❌  cEmail       → CustomerEmail
//   ❌  itms         → Items
//   ❌  addr         → DeliveryAddress
//   ❌  nm           → Name
//   ❌  qty          → Quantity
//   ❌  p            → UnitPrice
//   ❌  CS           → _connectionString
//   ❌  x            → activeItems
//   ❌  amt          → subtotal
//   ❌  fee          → shippingFee
//   ❌  tot          → total
//   ❌  c2           → itemCmd
//   ❌  em, s, log   → replaced by focused methods / Result<T>
//   ❌  Str, Cty     → Street, City  (Address properties)
//
// =============================================================================

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

// ----------------------------- Result<T> (Step 5) ----------------------------

public readonly record struct Result<T>
{
    public T?      Value     { get; init; }
    public string? Error     { get; init; }
    public bool    IsSuccess => Error is null;

    public static Result<T> Ok(T value)        => new() { Value = value };
    public static Result<T> Fail(string error) => new() { Error = error };
}

// ----------------------------- Domain model ----------------------------------

// Stretch goal: Address is now a record (immutable, equality-by-value)
public record Address(string Street, string City, string PostalCode);

public sealed class LineItem
{
    public string  Name      { get; init; } = "";
    public int     Quantity  { get; init; }
    public decimal UnitPrice { get; init; }
}

public sealed class Order
{
    public int          Id              { get; init; }
    public string       CustomerEmail   { get; init; } = "";
    public List<LineItem> Items         { get; init; } = [];
    public Address      DeliveryAddress { get; init; } = new Address("", "", "");
}

// ----------------------------- Pricing policy (Step 3) -----------------------

public static class PricingPolicy
{
    public const decimal StandardFeeRate  = 0.085m;   // 8.5 % handling fee
    public const decimal ExpressShipping  = 4.99m;    // flat express surcharge
}

// ----------------------------- Repository (Step 2) ---------------------------

public interface IOrderRepository
{
    Task SaveAsync(Order order, decimal subtotal, decimal fee, CancellationToken ct = default);
}

public sealed class SqlOrderRepository(string connectionString) : IOrderRepository
{
    public async Task SaveAsync(Order order, decimal subtotal, decimal fee, CancellationToken ct)
    {
        decimal total = subtotal + fee;

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        // ✅ parameterised queries — no SQL injection
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                "INSERT INTO Orders (Id, CustomerEmail, Total, Fee, ShippingStreet) " +
                "VALUES (@id, @email, @total, @fee, @street)";
            cmd.Parameters.AddWithValue("@id",     order.Id);
            cmd.Parameters.AddWithValue("@email",  order.CustomerEmail);
            cmd.Parameters.AddWithValue("@total",  total);
            cmd.Parameters.AddWithValue("@fee",    fee);
            cmd.Parameters.AddWithValue("@street", order.DeliveryAddress.Street);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        foreach (var item in order.Items.Where(i => i.Quantity > 0))
        {
            await using var itemCmd = conn.CreateCommand();
            itemCmd.CommandText =
                "INSERT INTO OrderItems (OrderId, Name, Quantity, UnitPrice) " +
                "VALUES (@orderId, @name, @qty, @price)";
            itemCmd.Parameters.AddWithValue("@orderId", order.Id);
            itemCmd.Parameters.AddWithValue("@name",    item.Name);
            itemCmd.Parameters.AddWithValue("@qty",     item.Quantity);
            itemCmd.Parameters.AddWithValue("@price",   item.UnitPrice);
            await itemCmd.ExecuteNonQueryAsync(ct);
        }
    }
}

// ----------------------------- Notification interfaces -----------------------

public interface IEmailNotificationService
{
    Task SendOrderConfirmationAsync(Order order, decimal total, CancellationToken ct = default);
}

public interface ISmsNotificationService
{
    Task SendOrderConfirmationAsync(Order order, CancellationToken ct = default);
}

public interface IAuditLogger
{
    Task LogOrderPlacedAsync(Order order, decimal total, CancellationToken ct = default);
}

// ----------------------------- Implementations -------------------------------

public sealed class SmtpEmailNotificationService(string smtpHost) : IEmailNotificationService
{
    public async Task SendOrderConfirmationAsync(Order order, decimal total, CancellationToken ct)
    {
        var smtp = new SmtpClient(smtpHost);
        await smtp.SendMailAsync(
            new MailMessage(
                "noreply@shop.com",
                order.CustomerEmail,
                $"Your order #{order.Id}",
                $"Total: {total:C}"),
            ct);
    }
}

public sealed class FileQueueSmsService(string queuePath) : ISmsNotificationService
{
    public async Task SendOrderConfirmationAsync(Order order, CancellationToken ct)
        => await File.AppendAllTextAsync(
            queuePath,
            $"{order.CustomerEmail}|Order #{order.Id} placed\n",
            ct);
}

public sealed class FileAuditLogger(string logPath) : IAuditLogger
{
    public async Task LogOrderPlacedAsync(Order order, decimal total, CancellationToken ct)
        => await File.AppendAllTextAsync(
            logPath,
            $"{DateTime.UtcNow:O} | order {order.Id} | {total:C}\n",  // ✅ UTC
            ct);
}

// ----------------------------- Order validator --------------------------------

public sealed class OrderValidator
{
    public Result<Order> Validate(Order order)
    {
        if (order is null)
            return Result<Order>.Fail("Order must not be null.");
        if (order.Items.Count == 0)
            return Result<Order>.Fail("Order must contain at least one item.");
        if (string.IsNullOrWhiteSpace(order.CustomerEmail))
            return Result<Order>.Fail("Customer email is required.");
        return Result<Order>.Ok(order);
    }
}

// ----------------------------- Service (Step 4 + 5) --------------------------

public sealed class OrderService(
    OrderValidator             validator,
    IOrderRepository           repository,
    IEmailNotificationService  emailNotifier,
    ISmsNotificationService    smsNotifier,
    IAuditLogger               auditLogger)
{
    // ✅ Result<T> return type — no out parameter, no flags
    public async Task<Result<Order>> ProcessAsync(
        Order order,
        bool  sendEmail,
        bool  sendSms,
        CancellationToken ct = default)
    {
        var validation = validator.Validate(order);
        if (!validation.IsSuccess)
            return validation;

        var (subtotal, fee) = CalculatePricing(order);

        await repository.SaveAsync(order, subtotal, fee, ct);

        if (sendEmail)
            await NotifyByEmailAsync(order, subtotal + fee, ct);

        if (sendSms)
            await NotifyBySmsAsync(order, ct);

        await auditLogger.LogOrderPlacedAsync(order, subtotal + fee, ct);

        return Result<Order>.Ok(order);
    }

    // ✅ Each method does exactly one thing
    private static (decimal subtotal, decimal fee) CalculatePricing(Order order)
    {
        var activeItems = order.Items.Where(i => i.Quantity > 0).ToList();
        decimal subtotal    = activeItems.Sum(i => i.UnitPrice * i.Quantity);
        decimal fee         = subtotal * PricingPolicy.StandardFeeRate;
        return (subtotal, fee);
    }

    private static (decimal subtotal, decimal fee) CalculateExpressPricing(Order order)
    {
        var (subtotal, fee) = CalculatePricing(order);
        return (subtotal, fee + PricingPolicy.ExpressShipping);
    }

    private async Task NotifyByEmailAsync(Order order, decimal total, CancellationToken ct)
        => await emailNotifier.SendOrderConfirmationAsync(order, total, ct);

    private async Task NotifyBySmsAsync(Order order, CancellationToken ct)
        => await smsNotifier.SendOrderConfirmationAsync(order, ct);
}
