using Mediator;
using Microsoft.Extensions.Logging;

namespace HandsOn021.Application;

// =============================================================================
// Task 3 Solution — LoggingBehavior (Mediator Pipeline Behavior)
// =============================================================================

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IMessage
{
    public async ValueTask<TResponse> Handle(
        TRequest request,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {Request}", typeof(TRequest).Name);
        var response = await next(request, cancellationToken);
        logger.LogInformation("Handled {Request}", typeof(TRequest).Name);
        return response;
    }
}
