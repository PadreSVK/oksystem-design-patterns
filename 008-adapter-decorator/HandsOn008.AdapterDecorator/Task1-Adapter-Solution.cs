// =============================================================================
// Hands-On 008 — Task 1: IPaymentGateway Adapter  (solution)
// =============================================================================

namespace HandsOn.AdapterDecorator;

public interface IPaymentGateway
{
    Task<string> ChargeAsync(decimal amount, string currency, string token,
                             CancellationToken ct = default);

    Task RefundAsync(string chargeId, decimal amount,
                     CancellationToken ct = default);
}

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

// ✅ Adapter — translates IPaymentGateway calls into FakePaymentSdk calls
public sealed class FakePaymentAdapter(FakePaymentSdk sdk) : IPaymentGateway
{
    public Task<string> ChargeAsync(decimal amount, string currency, string token,
                                    CancellationToken ct = default)
    {
        var chargeId = sdk.CreatePayment((long)(amount * 100), currency, token);
        return Task.FromResult(chargeId);
    }

    public Task RefundAsync(string chargeId, decimal amount,
                            CancellationToken ct = default)
    {
        sdk.IssueRefund(chargeId, (long)(amount * 100));
        return Task.CompletedTask;
    }
}

public sealed class OrderService(IPaymentGateway gateway)
{
    public async Task<string> CheckoutAsync(decimal total, string currency,
                                            string paymentToken,
                                            CancellationToken ct = default)
    {
        var chargeId = await gateway.ChargeAsync(total, currency, paymentToken, ct);
        return chargeId;
    }

    public async Task RefundOrderAsync(string chargeId, decimal amount,
                                       CancellationToken ct = default)
        => await gateway.RefundAsync(chargeId, amount, ct);
}
