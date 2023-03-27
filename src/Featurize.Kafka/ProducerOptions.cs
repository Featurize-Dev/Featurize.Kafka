using Confluent.Kafka;

namespace Kafka;


/// <summary>
/// Producer options class to configure a producer
/// </summary>
public sealed class ProducerOptions : ProducerConfig
{
    /// <summary>
    /// creates a new instance of the producer options.
    /// </summary>
    /// <param name="keySerializer">The serializer used to serialize the partition key.</param>
    /// <param name="valueSerializer">The serializer used to serialize the value.</param>
    public ProducerOptions(Type keySerializer, Type valueSerializer)
    {
        KeySerializer = keySerializer;
        ValueSerializer = valueSerializer;
    }

    /// <summary>
    /// The name of the topic that this producer produce to.
    /// </summary>
    public string? Topic { get; set; }
    /// <summary>
    /// The Key Serializer used to serialize the partition key.
    /// </summary>
    public Type KeySerializer { get; set; }
    /// <summary>
    /// The Value serializer used to serialize the message.
    /// </summary>
    public Type ValueSerializer { get; set; }

    /// <summary>
    /// The handle called when a error accourd during producing.
    /// </summary>
    public ErrorHandler ErrorHandler { get; set; } = DefaultLogHandlers.HandleErrors;
    /// <summary>
    /// The log handler used to log any information.
    /// </summary>
    public LogHandler LogHandler { get; set; } = DefaultLogHandlers.HandleLogMessage;
}
