using HandsOn021.Application;
using MediatR;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HandsOn021.Tests;

public class LoggingBehaviorTests
{
    private class InMemoryLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }

    [Fact]
    public async Task Handle_logs_before_and_after()
    {
        var logger = new InMemoryLogger<LoggingBehavior<PlaceOrderCommand, Result<Guid>>>();
        var behavior = new LoggingBehavior<PlaceOrderCommand, Result<Guid>>(logger);

        var result = await behavior.Handle(
            new PlaceOrderCommand("CUST-1", [new OrderItemDto("Widget", 1, 5.00m)]),
            () => Task.FromResult(Result<Guid>.Ok(Guid.NewGuid())),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, logger.Messages.Count);
        Assert.Contains("Handling", logger.Messages[0]);
        Assert.Contains("PlaceOrderCommand", logger.Messages[0]);
        Assert.Contains("Handled", logger.Messages[1]);
        Assert.Contains("PlaceOrderCommand", logger.Messages[1]);
    }

    [Fact]
    public async Task Handle_calls_next_delegate()
    {
        var logger = new InMemoryLogger<LoggingBehavior<GetOrderSummaryQuery, Result<OrderSummaryDto>>>();
        var behavior = new LoggingBehavior<GetOrderSummaryQuery, Result<OrderSummaryDto>>(logger);
        var nextCalled = false;

        var dto = new OrderSummaryDto(Guid.NewGuid(), "Placed", 10m, 1, DateTimeOffset.UtcNow);

        await behavior.Handle(
            new GetOrderSummaryQuery(Guid.NewGuid()),
            () =>
            {
                nextCalled = true;
                return Task.FromResult(Result<OrderSummaryDto>.Ok(dto));
            },
            CancellationToken.None);

        Assert.True(nextCalled);
    }
}
