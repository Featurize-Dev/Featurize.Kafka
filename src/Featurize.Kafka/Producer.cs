using Confluent.Kafka;
using Kafka;
using Microsoft.Extensions.Logging;

namespace Featurize.Kafka;

/// <summary>
/// A Kafka producer wrapper
/// </summary>
/// <typeparam name="TKey">The Key of the message.</typeparam>
/// <typeparam name="TValue">The Value of the message.</typeparam>
public sealed class Producer<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly ProducerOptions _options;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="producer"></param>
    /// <param name="options"></param>
    public Producer(IProducer<TKey, TValue> producer, ProducerOptions options)
    {
        _producer = producer;
        _options = options;
    }

    /// <summary>
    /// Produce a message on the configured topic.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ProduceAsync(TKey key, TValue value, CancellationToken cancellationToken = default) =>
        _producer.ProduceAsync(_options.Topic, new Message<TKey, TValue>()
        {
            Key = key,
            Value = value,
        }, cancellationToken);

    /// <summary>
    /// Produces a message on the configured topic
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ProduceAsync(Message<TKey, TValue> message, CancellationToken cancellationToken = default) =>
        _producer.ProduceAsync(_options.Topic, message, cancellationToken);
}
