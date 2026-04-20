// ============================================================
// Hands-On 002 — ServiceCollectionExtensions.cs
//
// TODO: Register all extracted interfaces and implementations
//       as part of Step 5 of the refactoring exercise.
// ============================================================

using Microsoft.Extensions.DependencyInjection;

namespace HandsOn.SolidRefactoring;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserAccountServices(
        this IServiceCollection services)
    {
        // TODO Step 4 — register IUserRepository
        // services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        // TODO Step 2 — register IEmailNotificationService
        // services.AddSingleton<IEmailNotificationService, SmtpEmailNotificationService>();

        // TODO Step 3 — register IPermissionStrategy implementations
        // services.AddSingleton<IPermissionStrategy, AdminPermissionStrategy>();
        // services.AddSingleton<IPermissionStrategy, ManagerPermissionStrategy>();

        // TODO — register the refactored UserAccountService itself
        // services.AddScoped<UserAccountService>();

        return services;
    }
}
