using Confluent.Kafka;

namespace Kafka;

public sealed class ConsumerOptions : ConsumerConfig
{
    public ConsumerOptions(Type keyDeserializer, Type valueDeserializer)
    {
        KeyDeserializer = keyDeserializer;
        ValueDeserializer = valueDeserializer;
    }
    public string Topic { get; set; } = string.Empty;
    public Type KeyDeserializer { get; set; }
    public Type ValueDeserializer { get; set; }
    public ErrorHandler ErrorHandler { get; set; } = DefaultLogHandlers.HandleErrors;
    public LogHandler LogHandler { get; set; } = DefaultLogHandlers.HandleLogMessage;
}
