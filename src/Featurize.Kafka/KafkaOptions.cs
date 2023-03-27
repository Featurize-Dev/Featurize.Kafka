using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Kafka;

/// <summary>
/// Kafka options
/// </summary>
public class KafkaOptions
{
    /// <summary>
    /// The kafka bootstrap servers
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:9092";

    /// <summary>
    /// The socket time in miliseconds.
    /// </summary>
    public int SocketTimeoutMs { get; set; } = 60000;
    /// <summary>
    /// The consumer group.
    /// </summary>
    public string GroupId { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// AutoOffsetReset
    /// </summary>
    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

    /// <summary>
    /// Collection of Consumer to use.
    /// </summary>
    public IConsumerCollection Consumers { get; } = new ConsumerCollection();

    /// <summary>
    /// Collection of Producers to use.
    /// </summary>
    public IProducerCollection Producers { get; } = new ProducerCollection();

    /// <summary>
    /// The Json Serializer settings to use with the serializer
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions();
}

/// <summary>
/// Extensions for kafka options for easier adding consumers and producers
/// </summary>
public static class KafkaOptionsExtensions
{

    /// <summary>
    /// Adds a consumer to the Consumer
    /// </summary>
    /// <typeparam name="THandler">The handler called when a message is consumed</typeparam>
    /// <typeparam name="TKey">The type of the partition key.</typeparam>
    /// <typeparam name="TValue">The type of the message</typeparam>
    /// <param name="s">Instance of kafka options</param>
    /// <param name="options">The options specific to this consumer</param>
    public static void AddConsumer<THandler, TKey, TValue>(this KafkaOptions s, Action<ConsumerOptions>? options = null)
        where THandler : IConsumerHandler<TKey, TValue>
    {
        var o = new ConsumerOptions(typeof(KafkaDeserializer<TKey>), typeof(KafkaDeserializer<TValue>))
        {
            BootstrapServers = s.BootstrapServers,
            SocketTimeoutMs = s.SocketTimeoutMs,
            GroupId = s.GroupId,
            AutoOffsetReset = s.AutoOffsetReset
        };
        options?.Invoke(o);
        s.Consumers.Add<THandler, TKey, TValue>(o);
    }

    /// <summary>
    /// Adds a producer to the feature
    /// </summary>
    /// <typeparam name="TKey">The partition key type.</typeparam>
    /// <typeparam name="TValue">The message type produced.</typeparam>
    /// <param name="s">Instance of the kafla options.</param>
    /// <param name="options">The options specific to the producer.</param>
    public static void AddProducer<TKey, TValue>(this KafkaOptions s, Action<ProducerOptions>? options = null)
    {
        var o = new ProducerOptions(typeof(KafkaSerializer<TKey>), typeof(KafkaSerializer<TValue>))
        {
            BootstrapServers = s.BootstrapServers,
            SocketTimeoutMs = s.SocketTimeoutMs,
        };

        options?.Invoke(o);
        s.Producers.Add<TKey, TValue>(o);
    }
}
