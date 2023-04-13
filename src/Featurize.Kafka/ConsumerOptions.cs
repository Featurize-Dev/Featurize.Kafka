using Confluent.Kafka;
using System.Text.Json;

namespace Kafka;

/// <summary>
/// Options used for this consumer.
/// </summary>
/// <summary>
/// Options for configuring a consumer.
/// </summary>
public sealed class ConsumerOptions : ConsumerConfig
{
    /// <summary>
    /// Creates a new instance of the consumer options
    /// </summary>
    /// <param name="keyDeserializer">The deserialzier used to deserialize the partition key.</param>
    /// <param name="valueDeserializer">The deserializer used to deserialize the message.</param>
    public ConsumerOptions(Type keyDeserializer, Type valueDeserializer)
    {
        KeyDeserializer = keyDeserializer;
        ValueDeserializer = valueDeserializer;
    }

    /// <summary>
    /// Name of the topic to consume.
    /// </summary>
    /// <summary>
    /// The name of the Topic to consume.
    /// </summary>
    public string Topic { get; set; } = string.Empty;
    /// <summary>
    /// The deserialzier used to deserialize the partition key.
    /// </summary>
    public Type KeyDeserializer { get; set; }
    /// <summary>
    /// The deserializer used to deserialize the message.
    /// </summary>
    public Type ValueDeserializer { get; set; }
    /// <summary>
    /// The error handler delegate to log errors.
    /// </summary>
    /// <summary>
    /// Delegate for handling errors.
    /// </summary>
    public ErrorHandler ErrorHandler { get; set; } = DefaultLogHandlers.HandleErrors;

    /// <summary>
    /// The log handler delegate to log messages.
    /// </summary>
    /// <summary>
    /// Delegate for handling log messages.
    /// </summary>
    public LogHandler LogHandler { get; set; } = DefaultLogHandlers.HandleLogMessage;

    /// <summary>
    /// The json serialization options.
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get;set; } = new(JsonSerializerDefaults.Web);
}
