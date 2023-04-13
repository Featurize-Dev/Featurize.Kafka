using Confluent.Kafka;
using System.Text.Json;

namespace Kafka;

/// <summary>
/// Options to configure the producer.
/// </summary>
public sealed class ProducerOptions : ProducerConfig
{
    /// <summary>
    /// Name of the topic to produce to.
    /// </summary>
    public string Topic { get; set; } = string.Empty;
    /// <summary>
    /// Delegate for handling errors.
    /// </summary>
    public ErrorHandler ErrorHandler { get; set; } = DefaultLogHandlers.HandleErrors;
    /// <summary>
    /// Delegate for handling log messages.
    /// </summary>
    public LogHandler LogHandler { get; set; } = DefaultLogHandlers.HandleLogMessage;

    /// <summary>
    /// Serialization options for the producers
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new(JsonSerializerDefaults.Web);
}
