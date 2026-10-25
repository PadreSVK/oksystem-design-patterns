// ============================================================
// Hands-On 002 — SOLID Refactoring — SOLUTION
// ============================================================

using System;
using System.Collections.Generic;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace HandsOn.SolidRefactoring.Solution;

// ============================================================
// Domain models — LSP violation fixed
// ============================================================

public abstract class User
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    // ✅ LSP: base contract — all subtypes honour this without throwing
    public virtual void Deactivate() => IsActive = false;

    public virtual decimal GetDiscount() => 0m;
}

public sealed class StandardUser : User { }

// ✅ LSP fixed: PremiumUser no longer throws — it sets a flag and triggers
//    cancellation logic instead of violating the base contract.
public sealed class PremiumUser : User
{
    public string SubscriptionTier { get; init; } = "Gold";
    public bool SubscriptionCancellationPending { get; private set; }

    public override void Deactivate()
    {
        SubscriptionCancellationPending = true;
        base.Deactivate(); // ✅ honours the base contract
    }

    public override decimal GetDiscount() => 0.20m;
}

// ============================================================
// Step 2 — SRP: extract email concern (IEmailNotificationService)
// ============================================================

public interface IEmailNotificationService
{
    void SendWelcome(string toEmail);
    void SendPasswordReset(string toEmail);
}

public sealed class SmtpEmailNotificationService : IEmailNotificationService
{
    private readonly SmtpClient _smtp = new("smtp.example.com", 587);
    private const string From = "noreply@example.com";

    public void SendWelcome(string toEmail) =>
        _smtp.Send(new MailMessage(From, toEmail, "Welcome!", "Your account has been created."));

    public void SendPasswordReset(string toEmail) =>
        _smtp.Send(new MailMessage(From, toEmail, "Password Reset", "Click the link to reset your password."));
}

// ============================================================
// Step 3 — OCP: replace role if/else with IPermissionStrategy
// ============================================================

public interface IPermissionStrategy
{
    bool CanHandle(string role);
    bool IsAllowed(string action);
}

public sealed class AdminPermissionStrategy : IPermissionStrategy
{
    public bool CanHandle(string role) => role == "Admin";
    public bool IsAllowed(string action) => true; // Admin can do everything
}

public sealed class ManagerPermissionStrategy : IPermissionStrategy
{
    public bool CanHandle(string role) => role == "Manager";
    public bool IsAllowed(string action) => action is "read" or "write";
}

public sealed class GuestPermissionStrategy : IPermissionStrategy
{
    public bool CanHandle(string role) => role == "Guest";
    public bool IsAllowed(string action) => action == "read";
}

// ✅ Stretch goal: add a third strategy without touching existing code — OCP proven
public sealed class AuditorPermissionStrategy : IPermissionStrategy
{
    public bool CanHandle(string role) => role == "Auditor";
    public bool IsAllowed(string action) => action is "read" or "export";
}

// ============================================================
// Step 4 — DIP: extract IUserRepository
// ============================================================

public interface IUserRepository
{
    User? FindByEmail(string email);
    void Add(User user);
    bool Exists(string email);
}

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _store = new();

    public User? FindByEmail(string email) =>
        _store.TryGetValue(email, out var u) ? u : null;

    public void Add(User user) => _store[user.Email] = user;

    public bool Exists(string email) => _store.ContainsKey(email);
}

// ============================================================
// Refactored UserAccountService — one reason to change (orchestration)
// ============================================================

public sealed class UserAccountService(
    IUserRepository repository,
    IEmailNotificationService email,
    IEnumerable<IPermissionStrategy> permissionStrategies)
{
    // ✅ SRP: only orchestrates — delegates each concern to its owner
    public void RegisterUser(string userEmail, string passwordHash, string role)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("Email is required.", nameof(userEmail));

        if (repository.Exists(userEmail))
            throw new InvalidOperationException($"User '{userEmail}' already exists.");

        repository.Add(new StandardUser
        {
            Email = userEmail,
            PasswordHash = passwordHash,
            Role = role
        });

        email.SendWelcome(userEmail); // ✅ SRP: email delegated
    }

    public bool Login(string userEmail, string passwordHash)
    {
        var user = repository.FindByEmail(userEmail);
        return user is { IsActive: true } && user.PasswordHash == passwordHash;
    }

    // ✅ OCP: new roles are handled by adding a new IPermissionStrategy
    public bool HasPermission(User user, string action)
    {
        var strategy = permissionStrategies.FirstOrDefault(s => s.CanHandle(user.Role));
        return strategy?.IsAllowed(action) ?? false;
    }

    public void SendPasswordReset(string userEmail)
    {
        if (!repository.Exists(userEmail))
            throw new InvalidOperationException("User not found.");

        email.SendPasswordReset(userEmail); // ✅ SRP: email delegated
    }

    public void DeactivateUser(string userEmail)
    {
        var user = repository.FindByEmail(userEmail)
            ?? throw new InvalidOperationException("User not found.");

        user.Deactivate(); // ✅ LSP: works correctly for all User subtypes
    }
}

// ============================================================
// Step 5 — DI registration
// ============================================================

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserAccountServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IEmailNotificationService, SmtpEmailNotificationService>();

        services.AddSingleton<IPermissionStrategy, AdminPermissionStrategy>();
        services.AddSingleton<IPermissionStrategy, ManagerPermissionStrategy>();
        services.AddSingleton<IPermissionStrategy, GuestPermissionStrategy>();
        // ✅ Stretch: registered without modifying any existing class — OCP proven
        services.AddSingleton<IPermissionStrategy, AuditorPermissionStrategy>();
    
        services.AddScoped<UserAccountService>();

        return services;
    }
}
