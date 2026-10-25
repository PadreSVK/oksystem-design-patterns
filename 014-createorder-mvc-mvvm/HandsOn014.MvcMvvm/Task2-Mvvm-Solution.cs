using System.ComponentModel;
using System.Windows.Input;

namespace HandsOn.MvcMvvm;

// ──── Gateway contract ───────────────────────────────────────────────

public interface ICreateOrderGateway
{
    Task CreateAsync(string customerId, CancellationToken ct = default);
}

// ──── AsyncRelayCommand ──────────────────────────────────────────────

public class AsyncRelayCommand : ICommand
{
    private readonly Func<CancellationToken, Task> _execute;
    private readonly Func<bool>? _canExecute;

    public AsyncRelayCommand(Func<CancellationToken, Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool IsRunning { get; private set; }

    public bool CanExecute(object? parameter) => !IsRunning && (_canExecute?.Invoke() ?? true);

    public async void Execute(object? parameter) => await ExecuteAsync();

    public async Task ExecuteAsync()
    {
        IsRunning = true;
        NotifyCanExecuteChanged();
        try
        {
            await _execute(CancellationToken.None);
        }
        finally
        {
            IsRunning = false;
            NotifyCanExecuteChanged();
        }
    }

    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

// ──── ViewModel ──────────────────────────────────────────────────────

public class CreateOrderViewModel : INotifyPropertyChanged
{
    private readonly ICreateOrderGateway _gateway;
    private string _customerId = "";
    private bool _isBusy;

    public CreateOrderViewModel(ICreateOrderGateway gateway)
    {
        _gateway = gateway;
        SubmitCommand = new AsyncRelayCommand(SubmitAsync, CanSubmit);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string CustomerId
    {
        get => _customerId;
        set { _customerId = value; OnPropertyChanged(nameof(CustomerId)); SubmitCommand.NotifyCanExecuteChanged(); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); SubmitCommand.NotifyCanExecuteChanged(); }
    }

    public AsyncRelayCommand SubmitCommand { get; }

    private bool CanSubmit() => !IsBusy && !string.IsNullOrWhiteSpace(CustomerId);

    private async Task SubmitAsync(CancellationToken ct)
    {
        IsBusy = true;
        try
        {
            await _gateway.CreateAsync(CustomerId, ct);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
