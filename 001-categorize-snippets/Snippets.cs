// ============================================================
// Hands-On 001 — Categorize Code Snippets
//
// Instructions (answer in answers.md):
//   1. Which CATEGORY does the snippet belong to?
//      (creational / structural / behavioral) — use the
//      Pattern Classification table from the slides.
//   2. What is the snippet TRYING to do?
//      (You do NOT need to name the exact GoF pattern.)
//   3. Does anything look wrong to you? Flag it from your
//      own C# experience.
//
// Be prepared to explain Snippet 3 — it contains a subtle violation.
// ============================================================

using System;

namespace HandsOn.CategorizeSnippets;

// ============================================================
// Snippet 1 — Which category? Anything you would flag?
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

// Central place that decides which processor to build.
public static class PaymentFactory
{
    public static IPaymentProcessor Create(PaymentKind kind) => kind switch
    {
        PaymentKind.Card   => new CardProcessor(),
        PaymentKind.PayPal => new PayPalProcessor(),
        // Q: PaymentKind.BankTransfer is declared in the enum. What happens here?
        _ => throw new NotSupportedException($"No processor for {kind}")
    };
}

// ============================================================
// Snippet 2 — Which category? Anything you would flag?
// ============================================================

// Third-party library we cannot change:
public sealed class LegacyXmlExporter
{
    public string ExportToXml(string data) => $"<xml>{data}</xml>";
}

// Our application expects this shape:
public interface IReportExporter
{
    string Export(string data);
}

public class XmlExportAdapter : IReportExporter
{
    private readonly LegacyXmlExporter _legacy = new();

    public string Export(string data) => _legacy.ExportToXml(data);

    // Q: Why would exposing the wrapped library defeat the purpose here?
    public LegacyXmlExporter Inner => _legacy;
}

// ============================================================
// Snippet 3 — Which category? Explain the subtle issue.
// ============================================================

public enum ShippingSpeed { Standard, Express, Overnight }

public class ShippingCalculator
{
    // Picks a pricing rule at runtime based on `speed`.
    public decimal Calculate(ShippingSpeed speed, decimal weightKg)
    {
        if (speed == ShippingSpeed.Standard)  return weightKg * 1.0m;
        else if (speed == ShippingSpeed.Express)   return weightKg * 2.5m;
        else if (speed == ShippingSpeed.Overnight) return weightKg * 4.0m;
        else return weightKg; // Q: what does an unknown speed do to the price?
    }
}
