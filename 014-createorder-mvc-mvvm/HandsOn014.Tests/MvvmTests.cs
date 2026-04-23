using Xunit;
using HandsOn.MvcMvvm;

namespace HandsOn014.Tests;

public class MvvmTests
{
    // ── test double ──────────────────────────────────────────────────

    private sealed class ControllableGateway : ICreateOrderGateway
    {
        public TaskCompletionSource Tcs { get; } = new();
        public string? ReceivedCustomerId { get; private set; }

        public Task CreateAsync(string customerId, CancellationToken ct)
        {
            ReceivedCustomerId = customerId;
            return Tcs.Task;
        }
    }

    // ── tests ────────────────────────────────────────────────────────

    [Fact]
    public void CustomerId_Set_RaisesPropertyChanged()
    {
        var vm = new CreateOrderViewModel(new ControllableGateway());
        var raised = new List<string>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName!);

        vm.CustomerId = "CUST-1";

        Assert.Contains("CustomerId", raised);
    }

    [Fact]
    public void SubmitCommand_CanExecute_False_WhenCustomerIdIsBlank()
    {
        var vm = new CreateOrderViewModel(new ControllableGateway());

        Assert.False(vm.SubmitCommand.CanExecute(null));
    }

    [Fact]
    public async Task SubmitCommand_Execute_TogglesIsBusy_AndCallsGateway()
    {
        var gateway = new ControllableGateway();
        var vm = new CreateOrderViewModel(gateway) { CustomerId = "CUST-42" };

        // Act — start execution but don't complete the gateway yet
        var task = vm.SubmitCommand.ExecuteAsync();

        // Assert — mid-execution
        Assert.True(vm.IsBusy);
        Assert.False(vm.SubmitCommand.CanExecute(null));

        // Complete the gateway
        gateway.Tcs.SetResult();
        await task;

        // Assert — after execution
        Assert.False(vm.IsBusy);
        Assert.True(vm.SubmitCommand.CanExecute(null));
        Assert.Equal("CUST-42", gateway.ReceivedCustomerId);
    }
}
