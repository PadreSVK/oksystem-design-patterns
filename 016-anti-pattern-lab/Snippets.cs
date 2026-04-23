namespace HandsOn016.AntiPatternLab;

// =============================================================================
// Snippet 1 — The God Class
// Identify the anti-pattern and refactor using High Cohesion / SRP.
// =============================================================================

public class UserManager
{
    public void Authenticate(string username, string password) { /* ... */ }
    public void RegisterUser(string email, string password) { /* ... */ }
    public void SendWelcomeEmail(string email) { /* ... */ }
    public void GenerateUserReport() { /* ... */ }
    public void UploadProfilePicture(byte[] data) { /* ... */ }
    public void LogActivity(string message) { /* ... */ }
}

// =============================================================================
// Snippet 2 — Anemic Domain Model
// Identify the anti-pattern and refactor using Information Expert / Rich Domain.
// =============================================================================

public class OrderDto
{
    public int Id { get; set; }
    public List<OrderLineDto> Lines { get; set; } = [];
}

public class OrderLineDto
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class OrderService
{
    public decimal CalculateTotal(OrderDto order)
    {
        return order.Lines.Sum(l => l.Price * l.Quantity);
    }
}

// =============================================================================
// Snippet 3 — Conditional Complexity
// Identify the anti-pattern and refactor using Polymorphism / Strategy Pattern.
// =============================================================================

public class PaymentProcessor
{
    public void Process(string type, decimal amount)
    {
        switch (type.ToLower())
        {
            case "card":
                Console.WriteLine($"Processing card payment of {amount}");
                break;
            case "paypal":
                Console.WriteLine($"Processing PayPal payment of {amount}");
                break;
            case "crypto":
                Console.WriteLine($"Processing Crypto payment of {amount}");
                break;
            default:
                throw new NotSupportedException();
        }
    }
}

// =============================================================================
// Snippet 4 — Service Locator
// Identify the anti-pattern and refactor using Dependency Injection.
// =============================================================================

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();
    public static void Register<T>(T service) => _services[typeof(T)] = service!;
    public static T Resolve<T>() => (T)_services[typeof(T)];
}

public class CheckoutService
{
    public void Checkout(decimal amount)
    {
        var logger = ServiceLocator.Resolve<ILogger>();
        var gateway = ServiceLocator.Resolve<IPaymentGateway>();
        
        gateway.Pay(amount);
        logger.Log("Payment successful");
    }
}

public interface ILogger { void Log(string msg); }
public interface IPaymentGateway { void Pay(decimal amount); }

// =============================================================================
// Snippet 5 — Leaky Abstraction
// Identify the anti-pattern and refactor using Protected Variations / Repository.
// =============================================================================

public interface IOrderRepository
{
    void Save(object order);
    void ExecuteRawSql(string sql); // ❌ Why is this here?
    string GetConnectionString();   // ❌ Or this?
}
