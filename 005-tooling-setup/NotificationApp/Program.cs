// =============================================================================
// Hands-On 005 — Tooling Setup  (entry point)
//
// Steps:
//   1. Inspect Directory.Build.props in the parent folder (005-tooling-setup/).
//      It sits above this project and applies AnalysisMode=All to everything below.
//   2. Run: dotnet build
//      CA-rule warnings will appear in NotificationService.cs — note each ID.
//   3. Open 005-tooling-setup/Directory.Build.props and uncomment the <ItemGroup>
//      with StyleCop.Analyzers and Roslynator.Analyzers, then run: dotnet build again.
//      Observe the additional SA/RCS warnings.
//   4. Fix the top 5 violations annotated with ❌ in NotificationService.cs.
//   5. Run: dotnet format --verify-no-changes
//      Confirm exit code 0.
//   6. Run: ./format-check.ps1
//      This is the script you would add to your CI pipeline.
// =============================================================================

// Suppress analyzer noise on entry-point boilerplate — violations to fix are in NotificationService.cs
#pragma warning disable CA2007  // ConfigureAwait not required on top-level statements
#pragma warning disable CA1303  // Literal string in Console.WriteLine is intentional
using HandsOn.ToolingSetup;

var service = new NotificationService();
await service.SendAsync("alice@example.com", "Your order has shipped");
await service.SendAsync("bob@example.com", "Payment confirmed");

Console.WriteLine("\nNotification log:");
foreach (var entry in service.Log)
    Console.WriteLine($"  {entry}");
