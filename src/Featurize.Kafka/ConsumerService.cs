using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka;

internal class ConsumerService<TKey, TValue> : BackgroundService
{
    private readonly ILogger<ConsumerService<TKey, TValue>> _log;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly IServiceProvider _provider;

    public ConsumerService(
        ILogger<ConsumerService<TKey, TValue>> log, 
        IConsumer<TKey, TValue> consumer,
        IServiceProvider provider)
    {
        _log = log;
        _consumer = consumer;
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        await using var scope = _provider.CreateAsyncScope();

        while (!stoppingToken.IsCancellationRequested)
        {
            var handler = scope.ServiceProvider.GetRequiredService<IConsumerHandler<TKey, TValue>>();
            var consumeResult = _consumer.Consume(stoppingToken);

            _log.LogDebug("{Key} - {Value}", consumeResult.Message.Key, consumeResult.Message.Value);

            await handler.Handle(consumeResult);
            _consumer.Commit(consumeResult);
        }
    }

    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}
