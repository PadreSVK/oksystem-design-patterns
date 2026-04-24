using System.Reflection;
using Xunit;

namespace HandsOn017.Tests;

/// <summary>
/// Architecture tests — verify Clean Architecture dependency rules.
/// These tests work regardless of whether the student has completed the exercises.
/// </summary>
public class ArchitectureTests
{
    [Fact]
    public void Domain_Has_Zero_PackageReferences()
    {
        // Read the Domain .csproj and verify no PackageReference entries
        var csprojPath = Path.Combine(
            GetSolutionDir(), "OrderService.Domain", "OrderService.Domain.csproj");

        var content = File.ReadAllText(csprojPath);

        Assert.DoesNotContain("PackageReference", content);
    }

    [Fact]
    public void Domain_Does_Not_Reference_EntityFrameworkCore()
    {
        var domainAssembly = typeof(OrderService.Domain.Order).Assembly;
        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        Assert.DoesNotContain(referencedAssemblies,
            a => a.Name!.Contains("EntityFrameworkCore", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Domain_Does_Not_Reference_Infrastructure()
    {
        var domainAssembly = typeof(OrderService.Domain.Order).Assembly;
        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        Assert.DoesNotContain(referencedAssemblies,
            a => a.Name!.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Application_Does_Not_Reference_Infrastructure()
    {
        var appAssembly = typeof(OrderService.Application.CreateOrderCommand).Assembly;
        var referencedAssemblies = appAssembly.GetReferencedAssemblies();

        Assert.DoesNotContain(referencedAssemblies,
            a => a.Name!.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Application_References_Domain()
    {
        var appAssembly = typeof(OrderService.Application.CreateOrderCommand).Assembly;
        var referencedAssemblies = appAssembly.GetReferencedAssemblies();

        Assert.Contains(referencedAssemblies,
            a => a.Name!.Contains("Domain", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Infrastructure_References_Application()
    {
        var csprojPath = Path.Combine(
            GetSolutionDir(), "OrderService.Infrastructure", "OrderService.Infrastructure.csproj");

        var content = File.ReadAllText(csprojPath);

        Assert.Contains("OrderService.Application", content);
    }

    private static string GetSolutionDir()
    {
        // Walk up from the test assembly's bin folder to the solution root
        var dir = AppContext.BaseDirectory;
        while (dir is not null && !File.Exists(Path.Combine(dir, "HandsOn017.slnx")))
            dir = Directory.GetParent(dir)?.FullName;

        return dir ?? throw new InvalidOperationException(
            "Could not find solution directory. Ensure HandsOn017.slnx exists.");
    }
}
