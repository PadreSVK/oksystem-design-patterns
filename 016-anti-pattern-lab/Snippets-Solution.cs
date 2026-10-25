namespace HandsOn016.AntiPatternLab.Solutions;

// =============================================================================
// Snippet 1 Solution — High Cohesion (SRP)
// God Class split into focused services.
// =============================================================================

public class AuthService { public void Authenticate(string u, string p) { } }
public class UserRegistrationService { public void Register(string e, string p) { } }
public class EmailService { public void SendWelcome(string e) { } }
public class ReportService { public void GenerateUserReport() { } }
public class ProfilePictureService { public void Upload(byte[] d) { } }

// =============================================================================
// Snippet 2 Solution — Rich Domain Model
// Information Expert applied: Order calculates its own total.
// =============================================================================

public class Order(int id)
{
    public int Id { get; } = id;
    public List<OrderLine> Lines { get; } = [];

    public decimal CalculateTotal() => Lines.Sum(l => l.Price * l.Quantity);
}

public record OrderLine(decimal Price, int Quantity);

// =============================================================================
// Snippet 3 Solution — Polymorphism (Strategy)
// Conditional complexity replaced with Strategy pattern.
// =============================================================================

public interface IPaymentStrategy
{
    bool CanHandle(string type);
    void Process(decimal amount);
}

public class CardPayment : IPaymentStrategy
{
    public bool CanHandle(string type) => type == "card";
    public void Process(decimal amount) => Console.WriteLine($"Card: {amount}");
}

public class PaymentProcessor(IEnumerable<IPaymentStrategy> strategies)
{
    public void Process(string type, decimal amount)
    {
        var strategy = strategies.FirstOrDefault(s => s.CanHandle(type)) 
                       ?? throw new NotSupportedException();
        strategy.Process(amount);
    }
}

// =============================================================================
// Snippet 4 Solution — Dependency Injection
// Service Locator replaced with Constructor Injection (Primary Constructor).
// =============================================================================

public interface ILogger { void Log(string msg); }
public interface IPaymentGateway { void Pay(decimal amount); }

public class CheckoutService(ILogger logger, IPaymentGateway gateway)
{
    public void Checkout(decimal amount)
    {
        gateway.Pay(amount);
        logger.Log("Payment successful");
    }
}

// =============================================================================
// Snippet 5 Solution — Protected Variations (Clean Repository)
// Leaky abstraction removed; implementation details hidden.
// =============================================================================

public interface IOrderRepository
{
    void Save(Order order);
    // ❌ Raw SQL and Connection String removed. 
    // They belong in the SQL-specific implementation, not the interface.
}

public class SqlOrderRepository(string connectionString) : IOrderRepository
{
    public void Save(Order order) 
    {
        // Connection string and SQL logic live here, safely hidden.
    }
}
