using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HandsOn018.ResultPattern;

// =============================================================================
// Task 3 Solution — MediatR Pipeline Behavior for Result Logging
// =============================================================================

public class ResultLoggingBehavior<TRequest, TResponse>(
    ILogger<ResultLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (response.IsFailed)
        {
            var errors = string.Join("; ", response.Errors.Select(e => e.Message));
            logger.LogWarning("Request {RequestName} failed: {Errors}",
                typeof(TRequest).Name, errors);
        }

        return response;
    }
}
