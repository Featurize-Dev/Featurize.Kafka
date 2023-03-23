using Confluent.Kafka;

namespace Kafka;

public sealed class ProducerOptions : ProducerConfig
{
    public ProducerOptions(Type keySerializer, Type valueSerializer)
    {
        KeySerializer = keySerializer;
        ValueSerializer = valueSerializer;
    }

    public string? Topic { get; set; }
    public Type KeySerializer { get; set; }
    public Type ValueSerializer { get; set; }
    public ErrorHandler ErrorHandler { get; set; } = DefaultLogHandlers.HandleErrors;
    public LogHandler LogHandler { get; set; } = DefaultLogHandlers.HandleLogMessage;
}
