// =============================================================================
// NotificationService — intentional analyzer violations.
// After step 2, dotnet build will report CA warnings here.
// After step 3 (StyleCop added), SA warnings also appear.
// Your task: fix the violations annotated with ❌.
// =============================================================================
using System.Collections.Concurrent;

namespace HandsOn.ToolingSetup;

// ❌ SA1600 (StyleCop, step 3): public type is missing an XML documentation comment
public class NotificationService
{
    // ❌ CA1051: do not declare visible instance fields — use a property
    public string DefaultSender = "noreply@example.com";

    // ❌ IDE naming (.editorconfig): private field must use _camelCase prefix
    //    visible as a squiggle in VS/Rider once .editorconfig is in place
    private readonly ConcurrentBag<string> log = [];

    // ❌ SA1600 (StyleCop, step 3): public method is missing an XML documentation comment
    public async Task SendAsync(string recipient, string message)
    {
        // ❌ CA1062: validate arguments — neither parameter is checked for null/empty
        var entry = $"{DateTime.Now}: {recipient} — {message}";

        // ❌ CA1305: DateTime interpolation should specify IFormatProvider
        //    Fix: use DateTime.UtcNow and FormattableString.Invariant(...)
        log.Add(entry);

        await Task.Delay(1); // ❌ CA2007: add .ConfigureAwait(false)
    }

    // ❌ CA1822: FormatSubject does not access any instance member → mark as static
    // ❌ SA1600 (StyleCop, step 3): public method is missing an XML documentation comment
    public string FormatSubject(string prefix, string subject)
    {
        // ❌ CA1307: ToUpper() should specify a StringComparison or use ToUpperInvariant()
        return $"[{prefix.ToUpper()}] {subject}";
    }

    // ❌ CA1024: GetLog is a zero-argument method returning state — CA1024 says "use a property"
    //    Fix: remove this method; the Log property below is the correct API
    public IReadOnlyCollection<string> GetLog() => log;

    // Property already exists — Program.cs uses this
    public IReadOnlyCollection<string> Log => log;
}
