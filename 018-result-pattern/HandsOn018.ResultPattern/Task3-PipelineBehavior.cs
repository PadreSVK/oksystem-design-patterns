using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;

namespace HandsOn018.ResultPattern;

// =============================================================================
// Task 3 — Mediator Pipeline Behavior for Result Logging
// =============================================================================
//
// You have a Mediator request/handler pair that returns Result<Guid>.
// Add a pipeline behavior that intercepts EVERY request whose response is a
// Result and logs a warning when the result has failed.
//
// YOUR TASKS:
//   1. Implement ResultLoggingBehavior<TRequest, TResponse> : IPipelineBehavior
//      - Constrain TRequest : notnull, IMessage and TResponse : ResultBase.
//      - After calling next(request, ct), check if the result IsFailed.
//      - If failed, log a warning with the request type name and error messages.
//   2. Register the behavior in DI (the test will do this for you).
//
// HINT: FluentResults' Result and Result<T> both inherit from ResultBase.
//       Use typeof(TRequest).Name for the request type name.
//       Mediator behaviors return ValueTask<TResponse>; the `next` delegate
//       is MessageHandlerDelegate<TRequest, TResponse> — call next(request, ct).
// =============================================================================

/// <summary>
/// TODO: Implement IPipelineBehavior&lt;TRequest, TResponse&gt;.
///   - Constrain TRequest : notnull, IMessage and TResponse : ResultBase
///   - Call next(request, cancellationToken) to get the response
///   - If response.IsFailed, log a warning:
///       "Request {RequestName} failed: {Errors}"
///   - Return the response
/// </summary>
public class ResultLoggingBehavior<TRequest, TResponse>
    // TODO: implement IPipelineBehavior<TRequest, TResponse>
    // TODO: add where constraints (TRequest : notnull, IMessage; TResponse : ResultBase)
{
    // TODO: accept ILogger<ResultLoggingBehavior<TRequest, TResponse>> via constructor
    // TODO: implement Handle returning ValueTask<TResponse>
}
