// ============================================================
// Hands-On 001 — Categorize Code Snippets — SOLUTION
//
// Each snippet is categorizable from the Pattern Classification
// table alone. The "flag" answers rely only on general C#
// experience — no pattern internals are required at this point
// in the course.
// ============================================================

using System;

namespace HandsOn.CategorizeSnippets.Solution;

// ============================================================
// Snippet 1 — PaymentFactory
//
// CATEGORY : Creational
// DOING    : Centralizes the creation of IPaymentProcessor
//            instances (a simple/static factory).
// FLAG     : 1) Not open for extension — every new PaymentKind
//               forces editing this switch (OCP smell).
//            2) BankTransfer is a declared enum value but has no
//               case, so Create(PaymentKind.BankTransfer) throws
//               at runtime instead of being caught at compile time.
//
// FIX: register processors by kind (e.g. a dictionary / DI) so a
//      new kind is added without editing the factory, and make
//      the missing case impossible or explicit.
// ============================================================

public enum PaymentKind { Card, PayPal, BankTransfer }

public interface IPaymentProcessor
{
    void Pay(decimal amount);
}

public class CardProcessor : IPaymentProcessor
{
    public void Pay(decimal amount) => Console.WriteLine($"[Card] {amount:C}");
}

public class PayPalProcessor : IPaymentProcessor
{
    public void Pay(decimal amount) => Console.WriteLine($"[PayPal] {amount:C}");
}

public class BankTransferProcessor : IPaymentProcessor
{
    public void Pay(decimal amount) => Console.WriteLine($"[BankTransfer] {amount:C}");
}

// ✅ Extensible — add a kind by registering it, not by editing a switch
public static class PaymentFactory
{
    private static readonly System.Collections.Generic.Dictionary<PaymentKind, Func<IPaymentProcessor>> _registry = new()
    {
        [PaymentKind.Card]         = () => new CardProcessor(),
        [PaymentKind.PayPal]       = () => new PayPalProcessor(),
        [PaymentKind.BankTransfer] = () => new BankTransferProcessor(),
    };

    public static IPaymentProcessor Create(PaymentKind kind) =>
        _registry.TryGetValue(kind, out var factory)
            ? factory()
            : throw new NotSupportedException($"No processor for {kind}");
}

// ============================================================
// Snippet 2 — XmlExportAdapter
//
// CATEGORY : Structural
// DOING    : Adapts the third-party LegacyXmlExporter to the
//            app's IReportExporter interface (Adapter).
// FLAG     : The `Inner` property leaks the wrapped library to
//            callers. The whole point of an adapter is to isolate
//            the rest of the code from the adaptee; exposing it
//            lets callers bind to LegacyXmlExporter again, undoing
//            the isolation.
//
// FIX: drop `Inner`; keep the adaptee private behind the interface.
// ============================================================

public sealed class LegacyXmlExporter
{
    public string ExportToXml(string data) => $"<xml>{data}</xml>";
}

public interface IReportExporter
{
    string Export(string data);
}

// ✅ Isolates the adaptee — nothing leaks
public class XmlExportAdapter : IReportExporter
{
    private readonly LegacyXmlExporter _legacy = new();

    public string Export(string data) => _legacy.ExportToXml(data);
}

// ============================================================
// Snippet 3 — ShippingCalculator  (the subtle one)
//
// CATEGORY : Behavioral
// DOING    : Selects a pricing algorithm at runtime based on the
//            `speed` argument (the intent of Strategy).
// FLAG     : Two issues.
//            1) Design: algorithm selection is an if/else chain,
//               not polymorphism — every new ShippingSpeed edits
//               this method (conditional-complexity smell).
//            2) Correctness (subtle): the final `else` is a catch-
//               all. An unrecognized speed silently returns the
//               base rate (weight * 1.0) — the system undercharges
//               instead of failing. With an enum this looks safe,
//               but a cast/overflow value or a newly added member
//               slips straight into the wrong branch.
//
// FIX: make each speed its own rule (Strategy), or at minimum use
//      an exhaustive switch expression that throws on an unknown
//      value so the gap is loud, not silent.
// ============================================================

public enum ShippingSpeed { Standard, Express, Overnight }

// ✅ Exhaustive + loud on unknown input
public class ShippingCalculator
{
    public decimal Calculate(ShippingSpeed speed, decimal weightKg) => speed switch
    {
        ShippingSpeed.Standard  => weightKg * 1.0m,
        ShippingSpeed.Express   => weightKg * 2.5m,
        ShippingSpeed.Overnight => weightKg * 4.0m,
        _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, "Unknown shipping speed")
    };
}
