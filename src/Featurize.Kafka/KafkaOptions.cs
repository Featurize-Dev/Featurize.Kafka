using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Kafka;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public int SocketTimeoutMs { get; set; } = 60000;
    public string GroupId { get; set; } = Guid.NewGuid().ToString();
    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

    public IConsumerCollection Consumers { get; } = new ConsumerCollection();
    public IProducerCollection Producers { get; } = new ProducerCollection();
    public JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions();
}

public static class KafkaOptionsExtensions
{
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
