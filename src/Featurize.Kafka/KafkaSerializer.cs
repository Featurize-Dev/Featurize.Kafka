using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Kafka;

/// <summary>
/// Kafka System.Text.Json serializer.
/// </summary>
/// <typeparam name="T">Type to serialize</typeparam>
public sealed class KafkaJsonSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<KafkaJsonSerializer<T>> _logger;

    /// <summary>
    /// Create a new instance of the serializer
    /// </summary>
    /// <param name="options">Json Options to use.</param>
    public KafkaJsonSerializer(JsonSerializerOptions options, ILogger<KafkaJsonSerializer<T>> logger)
    {
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data, _options);
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

public interface IKafkaSerializerFactory
{
    ISerializer<T> CreateSerializer<T>();
    IDeserializer<T> CreateDeserializer<T>();
}

public class JsonSerializerFactory : IKafkaSerializerFactory
{

    public JsonSerializerFactory(ILoggerFactory loggerFactory, JsonSerializerOptions options)
    {
        LoggerFactory = loggerFactory;
        Options = options;
    }

    public JsonSerializerOptions Options { get; set; }

    public ILoggerFactory LoggerFactory { get; set; }

    public IDeserializer<T> CreateDeserializer<T>()
    {
        return new KafkaJsonSerializer<T>(Options, LoggerFactory.CreateLogger<KafkaJsonSerializer<T>>());
    }

    public ISerializer<T> CreateSerializer<T>()
    {
        return new KafkaJsonSerializer<T>(Options, LoggerFactory.CreateLogger<KafkaJsonSerializer<T>>());
    }
}