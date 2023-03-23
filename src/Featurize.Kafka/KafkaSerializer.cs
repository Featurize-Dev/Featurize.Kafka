using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Kafka;

internal class KafkaSerializer<T> : ISerializer<T>
{
    private readonly JsonSerializerOptions _options;

    public KafkaSerializer(JsonSerializerOptions options)
    {
        _options = options;
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data, _options);
    }
}

public class KafkaDeserializer<T> : IDeserializer<T>
{
    private readonly JsonSerializerOptions _options;
    private readonly ILogger _logger;

    public KafkaDeserializer(JsonSerializerOptions options, ILogger<KafkaDeserializer<T>> logger)
    {
        _options = options;
        _logger = logger;
    }
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        try
        {
            if (isNull)
            {
                return default!;
            }

            var result = JsonSerializer.Deserialize<T>(data, _options);

            return (result ?? default)!;
        }
        catch (JsonException ex)
        {
            var stringContent = Encoding.UTF8.GetString(data);
            _logger.LogError(ex, "Failed to deserialize: '{0}'.", stringContent);
            throw;
        }
    }
}
