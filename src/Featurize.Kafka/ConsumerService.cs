using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka;

/// <summary>
/// Consumer service
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class ConsumerService<TKey, TValue> : BackgroundService
{
    private readonly ILogger<ConsumerService<TKey, TValue>> _logger;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="consumer"></param>
    /// <param name="provider"></param>
    public ConsumerService(
        ILogger<ConsumerService<TKey, TValue>> logger, 
        IConsumer<TKey, TValue> consumer,
        IServiceProvider provider)
    {
        _logger = logger;
        _consumer = consumer;
        _provider = provider;
    }
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        await using var scope = _provider.CreateAsyncScope();

        while (!stoppingToken.IsCancellationRequested)
        {
            var handler = scope.ServiceProvider.GetRequiredService<IConsumerHandler<TKey, TValue>>();
            var consumeResult = _consumer.Consume(stoppingToken);

            _logger.LogDebug("{Key} - {Value}", consumeResult.Message.Key, consumeResult.Message.Value);

            await handler.Handle(consumeResult);
            _consumer.Commit(consumeResult);
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}
