using Mediator;
using Microsoft.Extensions.Logging;

namespace HandsOn021.Application;

// =============================================================================
// Task 3 — LoggingBehavior (Mediator Pipeline Behavior)
// =============================================================================
//
// A pipeline behavior wraps every Mediator request, allowing cross-cutting
// concerns (logging, validation, transactions) without modifying handlers.
//
// YOUR TASKS:
//   1. Implement IPipelineBehavior<TRequest, TResponse> (TRequest : notnull, IMessage).
//   2. Accept ILogger<LoggingBehavior<TRequest, TResponse>> via constructor.
//   3. In Handle (returns ValueTask<TResponse>):
//      a. Log "Handling {Request}" (typeof(TRequest).Name) at Information level
//      b. Call var response = await next(request, cancellationToken);
//      c. Log "Handled {Request}" (typeof(TRequest).Name) at Information level
//      d. Return response
//
// HINT: The pipeline behavior is registered in DI and automatically wraps
//       all Mediator handlers. The `next` delegate (MessageHandlerDelegate)
//       calls the actual handler.
// =============================================================================

public class LoggingBehavior<TRequest, TResponse>
    // TODO: implement IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IMessage
{
    // TODO: Accept ILogger via constructor

    // TODO: Implement Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken ct)
    //   1. _logger.LogInformation("Handling {Request}", typeof(TRequest).Name);
    //   2. var response = await next(request, ct);
    //   3. _logger.LogInformation("Handled {Request}", typeof(TRequest).Name);
    //   4. return response;
}
