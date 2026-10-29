using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;

namespace HandsOn018.ResultPattern;

// =============================================================================
// Task 3 Solution — Mediator Pipeline Behavior for Result Logging
// =============================================================================

public class ResultLoggingBehavior<TRequest, TResponse>(
    ILogger<ResultLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IMessage
    where TResponse : ResultBase
{
    public async ValueTask<TResponse> Handle(
        TRequest request,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(request, cancellationToken);

        if (response.IsFailed)
        {
            var errors = string.Join("; ", response.Errors.Select(e => e.Message));
            logger.LogWarning("Request {RequestName} failed: {Errors}",
                typeof(TRequest).Name, errors);
        }

        return response;
    }
}
