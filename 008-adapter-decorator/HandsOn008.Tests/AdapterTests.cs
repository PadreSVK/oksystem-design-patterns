// =============================================================================
// Hands-On 008 — Task 1 Tests: IPaymentGateway Adapter
//
// These tests are always compiled and run.
// Fix Task1-Adapter.cs in HandsOn008.AdapterDecorator/ until all pass.
// =============================================================================

using Xunit;
using HandsOn.AdapterDecorator;

namespace HandsOn008.Tests;

public class AdapterTests
{
    // ── Unit tests: OrderService depends only on IPaymentGateway ─────────────

    [Fact]
    public async Task OrderService_Checkout_CallsChargeAsync()
    {
        var spy = new SpyPaymentGateway();
        var service = new OrderService(spy);

        var chargeId = await service.CheckoutAsync(99.99m, "USD", "tok_test");

        Assert.Single(spy.Charges);
        Assert.Equal(99.99m,     spy.Charges[0].Amount);
        Assert.Equal("USD",      spy.Charges[0].Currency);
        Assert.Equal("tok_test", spy.Charges[0].Token);
        Assert.Equal(chargeId,   spy.Charges[0].ReturnedId);
    }

    [Fact]
    public async Task OrderService_Refund_CallsRefundAsync()
    {
        var spy = new SpyPaymentGateway();
        var service = new OrderService(spy);

        await service.RefundOrderAsync("chg_abc", 50m);

        Assert.Single(spy.Refunds);
        Assert.Equal("chg_abc", spy.Refunds[0].ChargeId);
        Assert.Equal(50m,       spy.Refunds[0].Amount);
    }

    // ── Integration tests: adapter translates correctly to the SDK ────────────

    [Fact]
    public async Task Adapter_ChargeAsync_TranslatesToSdkCreatePayment()
    {
        var sdk = new FakePaymentSdk();
        var adapter = new FakePaymentAdapter(sdk);

        var chargeId = await adapter.ChargeAsync(12.50m, "EUR", "tok_123");

        Assert.False(string.IsNullOrEmpty(chargeId));
        Assert.Single(sdk.Log);
        Assert.Contains("1250", sdk.Log[0]);  // 12.50 × 100 = 1250 cents
        Assert.Contains("EUR",  sdk.Log[0]);
        Assert.Contains("tok_123", sdk.Log[0]);
    }

    [Fact]
    public async Task Adapter_RefundAsync_TranslatesToSdkIssueRefund()
    {
        var sdk = new FakePaymentSdk();
        var adapter = new FakePaymentAdapter(sdk);

        var chargeId = await adapter.ChargeAsync(10m, "USD", "tok_abc");
        await adapter.RefundAsync(chargeId, 5m);

        Assert.Equal(2, sdk.Log.Count);
        Assert.Contains("500", sdk.Log[1]);   // 5.00 × 100 = 500 cents
        Assert.Contains(chargeId, sdk.Log[1]);
    }

    [Fact]
    public async Task Adapter_ChargeAsync_ReturnsSdkChargeId()
    {
        var sdk = new FakePaymentSdk();
        var adapter = new FakePaymentAdapter(sdk);

        var chargeId = await adapter.ChargeAsync(1m, "USD", "tok_x");

        Assert.StartsWith("chg_", chargeId);
    }

    // ── Spy / test double ─────────────────────────────────────────────────────

    private sealed record ChargeCall(decimal Amount, string Currency, string Token, string ReturnedId);
    private sealed record RefundCall(string ChargeId, decimal Amount);

    private sealed class SpyPaymentGateway : IPaymentGateway
    {
        public List<ChargeCall> Charges { get; } = [];
        public List<RefundCall> Refunds { get; } = [];

        public Task<string> ChargeAsync(decimal amount, string currency, string token,
                                        CancellationToken ct = default)
        {
            var id = $"spy_{Guid.NewGuid():N}";
            Charges.Add(new ChargeCall(amount, currency, token, id));
            return Task.FromResult(id);
        }

        public Task RefundAsync(string chargeId, decimal amount, CancellationToken ct = default)
        {
            Refunds.Add(new RefundCall(chargeId, amount));
            return Task.CompletedTask;
        }
    }
}
