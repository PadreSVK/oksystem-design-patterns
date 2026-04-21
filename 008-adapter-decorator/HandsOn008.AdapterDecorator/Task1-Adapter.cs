// =============================================================================
// Hands-On 008 — Task 1: IPaymentGateway Adapter  (starter)
//
// Scenario: your domain uses IPaymentGateway. FakePaymentSdk is a third-party
//           class with an incompatible API — you cannot modify it.
//
// Your task:
//   Implement FakePaymentAdapter so OrderService can work with FakePaymentSdk
//   without ever knowing the SDK exists.
//
// Hint: the SDK works in cents — multiply amounts by 100 and cast to long.
// =============================================================================

namespace HandsOn.AdapterDecorator;

// ─── Target interface (yours — your domain depends on this) ──────────────────

public interface IPaymentGateway
{
    Task<string> ChargeAsync(decimal amount, string currency, string token,
                             CancellationToken ct = default);

    Task RefundAsync(string chargeId, decimal amount,
                     CancellationToken ct = default);
}

// ─── Adaptee — third-party SDK you CANNOT modify ─────────────────────────────

public sealed class FakePaymentSdk
{
    private readonly List<string> _log = [];

    public IReadOnlyList<string> Log => _log;

    public string CreatePayment(long amountInCents, string currencyCode, string paymentToken)
    {
        var chargeId = $"chg_{Guid.NewGuid():N}";
        _log.Add($"CreatePayment({amountInCents}, {currencyCode}, {paymentToken}) → {chargeId}");
        return chargeId;
    }

    public void IssueRefund(string chargeRef, long amountInCents)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chargeRef);
        _log.Add($"IssueRefund({chargeRef}, {amountInCents})");
    }
}

// ─── Adapter (your task) ─────────────────────────────────────────────────────

public sealed class FakePaymentAdapter : IPaymentGateway
{
    private readonly FakePaymentSdk _sdk;

    public FakePaymentAdapter(FakePaymentSdk sdk) => _sdk = sdk;

    public Task<string> ChargeAsync(decimal amount, string currency, string token,
                                    CancellationToken ct = default)
    {
        // TODO: call _sdk.CreatePayment(amountInCents, currency, token)
        //       Hint: amountInCents = (long)(amount * 100)
        //       return Task.FromResult(chargeId)
        throw new NotImplementedException();
    }

    public Task RefundAsync(string chargeId, decimal amount,
                            CancellationToken ct = default)
    {
        // TODO: call _sdk.IssueRefund(chargeId, amountInCents)
        //       Hint: same cents conversion applies
        //       return Task.CompletedTask
        throw new NotImplementedException();
    }
}

// ─── Client (provided — do not modify) ───────────────────────────────────────

public sealed class OrderService(IPaymentGateway gateway)
{
    public async Task<string> CheckoutAsync(decimal total, string currency,
                                            string paymentToken,
                                            CancellationToken ct = default)
    {
        // ✅ OrderService never mentions FakePaymentSdk — only the interface
        var chargeId = await gateway.ChargeAsync(total, currency, paymentToken, ct);
        return chargeId;
    }

    public async Task RefundOrderAsync(string chargeId, decimal amount,
                                       CancellationToken ct = default)
        => await gateway.RefundAsync(chargeId, amount, ct);
}
