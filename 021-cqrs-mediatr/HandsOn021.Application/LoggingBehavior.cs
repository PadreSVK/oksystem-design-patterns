using MediatR;
using Microsoft.Extensions.Logging;

namespace HandsOn021.Application;

// =============================================================================
// Task 3 — LoggingBehavior (MediatR Pipeline Behavior)
// =============================================================================
//
// A pipeline behavior wraps every MediatR request, allowing cross-cutting
// concerns (logging, validation, transactions) without modifying handlers.
//
// YOUR TASKS:
//   1. Implement IPipelineBehavior<TRequest, TResponse>.
//   2. Accept ILogger<LoggingBehavior<TRequest, TResponse>> via constructor.
//   3. In Handle:
//      a. Log "Handling {Request}" (typeof(TRequest).Name) at Information level
//      b. Call var response = await next();
//      c. Log "Handled {Request}" (typeof(TRequest).Name) at Information level
//      d. Return response
//
// HINT: The pipeline behavior is registered in DI and automatically wraps
//       all MediatR handlers. The `next` delegate calls the actual handler.
// =============================================================================

public class LoggingBehavior<TRequest, TResponse>
    // TODO: implement IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    // TODO: Accept ILogger via constructor

    // TODO: Implement Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    //   1. _logger.LogInformation("Handling {Request}", typeof(TRequest).Name);
    //   2. var response = await next();
    //   3. _logger.LogInformation("Handled {Request}", typeof(TRequest).Name);
    //   4. return response;
}
