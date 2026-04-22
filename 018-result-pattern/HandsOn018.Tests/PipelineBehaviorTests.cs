using FluentResults;
using HandsOn018.ResultPattern;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HandsOn018.Tests;

public class PipelineBehaviorTests
{
    private IMediator BuildMediator()
    {
        var services = new ServiceCollection();

        services.AddLogging(b => b.AddProvider(new InMemoryLoggerProvider(LogStore)));
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<PlaceOrderMediatRHandler>();
            cfg.AddOpenBehavior(typeof(ResultLoggingBehavior<,>));
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IMediator>();
    }

    private List<string> LogStore { get; } = [];

    [Fact]
    public async Task SuccessfulRequest_NoWarningLogged()
    {
        var mediator = BuildMediator();
        var command = new PlaceOrderMediatRCommand("CUST-1",
        [
            new OrderItemDto("PROD-A", 1, 10.00m)
        ]);

        var result = await mediator.Send(command);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(LogStore, msg => msg.Contains("failed"));
    }

    [Fact]
    public async Task FailedRequest_WarningIsLogged()
    {
        var mediator = BuildMediator();
        var command = new PlaceOrderMediatRCommand("CUST-1", []);

        var result = await mediator.Send(command);

        Assert.True(result.IsFailed);
        Assert.Contains(LogStore, msg =>
            msg.Contains("PlaceOrderMediatRCommand") && msg.Contains("failed"));
    }

    [Fact]
    public async Task FailedRequest_ErrorMessageInLog()
    {
        var mediator = BuildMediator();
        var command = new PlaceOrderMediatRCommand("CUST-1", []);

        await mediator.Send(command);

        Assert.Contains(LogStore, msg => msg.Contains("at least one item"));
    }
}

// --- In-memory logger for test assertions ---

file class InMemoryLoggerProvider(List<string> store) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new InMemoryLogger(store);
    public void Dispose() { }
}

file class InMemoryLogger(List<string> store) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        store.Add(formatter(state, exception));
    }
}
