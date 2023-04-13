using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Kafka;

/// <summary>
/// The options to configure Kafka Feature
/// </summary>
public sealed class KafkaOptions
{
    /// <summary>
    /// The bootstrap service of Kafka
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:9092";
    /// <summary>
    /// The time out in miliseconds
    /// </summary>
    public int SocketTimeoutMs { get; set; } = 60000;
    /// <summary>
    /// The Consumer Group Id
    /// </summary>
    public string GroupId { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// Auto offset Reset
    /// </summary>
    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

    /// <summary>
    /// Collection of Consumers to register.
    /// </summary>
    public IConsumerCollection Consumers { get; } = new ConsumerCollection();
    /// <summary>
    /// Collection of Producers to register.
    /// </summary>
    public IProducerCollection Producers { get; } = new ProducerCollection();
    /// <summary>
    /// The global serialization options.
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions();
}

/// <summary>
/// Extension methods for configuring Kafka
/// </summary>
public static class KafkaOptionsExtensions
{

    /// <summary>
    /// Registers a Consumer
    /// </summary>
    /// <typeparam name="THandler">The handler that handles the message</typeparam>
    /// <param name="s">The kafka options to extend.</param>
    /// <param name="options">The consumer options</param>
    public static void AddConsumer<THandler>(this KafkaOptions s, Action<ConsumerOptions>? options = null)
    {
        var o = new ConsumerOptions()
        {
            BootstrapServers = s.BootstrapServers,
            SocketTimeoutMs = s.SocketTimeoutMs,
            GroupId = s.GroupId,
            AutoOffsetReset = s.AutoOffsetReset,
            JsonSerializerOptions = s.SerializerOptions,
        };
        options?.Invoke(o);
        s.Consumers.Add<THandler>(o);
    }

    /// <summary>
    /// Registers a Producer.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="s"></param>
    /// <param name="options"></param>
    public static void AddProducer<TKey, TValue>(this KafkaOptions s, Action<ProducerOptions>? options = null)
    {
        var o = new ProducerOptions()
        {
            BootstrapServers = s.BootstrapServers,
            SocketTimeoutMs = s.SocketTimeoutMs,
            JsonSerializerOptions = s.SerializerOptions,
        };

        options?.Invoke(o);
        s.Producers.Add<TKey, TValue>(o);
    }
}
