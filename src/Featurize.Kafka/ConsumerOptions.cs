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
    /// Name of the topic to consume.
    /// </summary>
    /// <summary>
    /// The name of the Topic to consume.
    /// </summary>
    public string Topic { get; set; } = string.Empty;
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
