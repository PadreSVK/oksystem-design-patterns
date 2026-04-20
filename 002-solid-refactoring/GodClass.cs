// ============================================================
// Hands-On 002 — SOLID Refactoring
//
// Instructions:
//   1. Read UserAccountService below — identify which SOLID
//      principles are violated and annotate with comments.
//   2. Extract email-sending logic into IEmailNotificationService
//      + implementation (SRP).
//   3. Replace the if (role == "Admin") / if (role == "Manager")
//      block with IPermissionStrategy and two implementations (OCP).
//   4. Introduce IUserRepository and update UserAccountService
//      to use constructor injection (DIP).
//   5. Register everything in ServiceCollectionExtensions.cs.
//   6. Run: dotnet test hands-on/002-solid-refactoring.Tests
//
// Stretch goal:
//   - Spot the hidden LSP violation in PremiumUser : User and fix it.
//   - Add a third IPermissionStrategy without touching existing code.
// ============================================================

using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace HandsOn.SolidRefactoring;

// --------------- Domain models --------------------------------

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; protected set; } = true;

    public virtual void Deactivate() => IsActive = false;

    public virtual decimal GetDiscount() => 0m;
}

// Q (stretch): What LSP violation is hiding here?
public class PremiumUser : User
{
    public string SubscriptionTier { get; set; } = "Gold";

    // Override intentionally breaks substitutability
    public override void Deactivate() =>
        throw new InvalidOperationException(
            "Premium users cannot be deactivated directly. Cancel the subscription first.");

    public override decimal GetDiscount() => 0.20m;
}

// --------------- The God Class --------------------------------
// Q: How many reasons does this class have to change?

public class UserAccountService
{
    // ❌ DIP: constructs its own low-level dependencies
    private readonly Dictionary<string, User> _store = new();
    private readonly SmtpClient _smtp = new("smtp.example.com", 587);

    // ❌ SRP: registration + persistence + email in one method
    public void RegisterUser(string email, string passwordHash, string role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (_store.ContainsKey(email))
            throw new InvalidOperationException($"User '{email}' already exists.");

        var user = new User { Email = email, PasswordHash = passwordHash, Role = role };
        _store[email] = user;

        // ❌ SRP: email concern leaks into registration
        var message = new MailMessage(
            from: "noreply@example.com",
            to: email,
            subject: "Welcome!",
            body: "Your account has been created.");

        _smtp.Send(message);
    }

    // ❌ SRP: authentication is a separate concern
    public bool Login(string email, string passwordHash)
    {
        if (!_store.TryGetValue(email, out var user))
            return false;

        return user.PasswordHash == passwordHash && user.IsActive;
    }

    // ❌ OCP: every new role requires editing this method
    public bool HasPermission(User user, string action)
    {
        if (user.Role == "Admin")
            return true;

        if (user.Role == "Manager")
            return action is "read" or "write";

        if (user.Role == "Guest")
            return action == "read";

        return false;
    }

    // ❌ SRP: email logic duplicated — divergent change waiting to happen
    public void SendPasswordReset(string email)
    {
        if (!_store.ContainsKey(email))
            throw new InvalidOperationException("User not found.");

        var message = new MailMessage(
            from: "noreply@example.com",
            to: email,
            subject: "Password Reset",
            body: "Click the link to reset your password.");

        _smtp.Send(message);
    }

    // ❌ SRP: deactivation + audit combined
    public void DeactivateUser(string email)
    {
        if (!_store.TryGetValue(email, out var user))
            throw new InvalidOperationException("User not found.");

        user.Deactivate(); // ❌ LSP: breaks if user is PremiumUser
        Console.WriteLine($"[AUDIT] User {email} deactivated at {DateTime.UtcNow}");
    }
}
