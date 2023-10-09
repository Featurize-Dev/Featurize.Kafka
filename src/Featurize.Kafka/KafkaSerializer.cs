using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Kafka;

/// <summary>
/// Kafka System.Text.Json serializer.
/// </summary>
/// <typeparam name="T">Type to serialize</typeparam>
public sealed class KafkaSerializer<T> : ISerializer<T>
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Create a new instance of the serializer
    /// </summary>
    /// <param name="options">Json Options to use.</param>
    public KafkaSerializer(JsonSerializerOptions options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data, _options);
    }
}

/// <summary>
/// Kafka System.Text.Json Deserializer.
/// </summary>
/// <typeparam name="T">Type to deserialize to.</typeparam>
public sealed class KafkaDeserializer<T> : IDeserializer<T>
{
    private readonly JsonSerializerOptions _options;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of the deserializer
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public KafkaDeserializer(JsonSerializerOptions options, ILogger<KafkaDeserializer<T>> logger)
    {
        _options = options;
        _logger = logger;
    }
    /// <inheritdoc />
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
            _logger.LogError(ex, "Failed to deserialize: '{0}'.", ex.Message);
            throw;
        }
    }
}
