using Confluent.Kafka;
using Featurize;
using Featurize.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Kafka;

/// <summary>
/// Adds AddKafka extension to the feature collection.
/// </summary>
public static class FeatureCollectionExtensions
{
    /// <summary>
    /// Adds kafka messaging feature to the feature collection
    /// </summary>
    /// <param name="service">The feature collection to add Kafka feature to.</param>
    /// <param name="options">The options needed to configure kafka.</param>
    /// <returns></returns>
    public static IFeatureCollection AddKafka(this IFeatureCollection service, Action<KafkaOptions>? options = null)
        => service.AddWithOptions<KafkaFeature, KafkaOptions>(options);
}


/// <summary>
/// Feature for enable Kafka Messaging
/// </summary>
public sealed class KafkaFeature : 
    IFeatureWithOptions<KafkaFeature, KafkaOptions>,
    IFeatureWithConfigurableOptions<KafkaOptions>,
    IServiceCollectionFeature
{
    private KafkaFeature(KafkaOptions options)
    {
        Options = options;
    }

    /// <inheritdoc />

    public KafkaOptions Options { get; }

    /// <inheritdoc />

    public static KafkaFeature Create(KafkaOptions config)
    {
        return new KafkaFeature(config);
    }

    /// <inheritdoc />
    public void Configure(IServiceCollection services)
    {
        RegisterConsumers(services);
        RegisterProducers(services);
    }

    private void RegisterProducers(IServiceCollection services)
    {
        foreach (var producer in Options.Producers)
        {
            var methodInfo = GetType().GetMethod(nameof(ProducerFactory), BindingFlags.Static | BindingFlags.NonPublic)!;
            var factory = methodInfo.MakeGenericMethod(producer.KeyType, producer.ValueType);
            factory.Invoke(this, new object[] { services, producer.Options });
        }
    }

    private static void ProducerFactory<TKey, TValue>(IServiceCollection services, ProducerOptions options)
    {
        services.AddSingleton<Confluent.Kafka.IProducer<TKey, TValue>>(s =>
        {
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<IProducer<TKey, TValue>>();
            var keySerializer = new KafkaSerializer<TKey>(options.JsonSerializerOptions);
            var valueSerializer = new KafkaSerializer<TValue>(options.JsonSerializerOptions);

            var producer = new ProducerBuilder<TKey, TValue>(options)
                        .SetKeySerializer(keySerializer)
                        .SetValueSerializer(valueSerializer)
                        .SetErrorHandler((p, e) => options.ErrorHandler(logger, p, e))
                        .SetLogHandler((p, m) => options.LogHandler(logger, p, m))
                        .Build();

            return producer;
        });

        services.AddSingleton(s => {
            var producer = s.GetRequiredService<IProducer<TKey, TValue>>();
            return new Producer<TKey, TValue>(producer, options); ;
        });
    }

    private void RegisterConsumers(IServiceCollection services)
    {
        foreach (var consumer in Options.Consumers)
        {
            var methodInfo = GetType().GetMethod(nameof(ConsumerFactory), BindingFlags.Static | BindingFlags.NonPublic)!;
            var factory = methodInfo.MakeGenericMethod(consumer.HandlerType, consumer.KeyType, consumer.ValueType);
            factory.Invoke(this, new object[] { services, consumer.Options });
        }
    }


    private static void ConsumerFactory<THandler, TKey, TValue>(IServiceCollection services, ConsumerOptions options)
        where THandler : class, IConsumerHandler<TKey, TValue>
    {
        services.AddSingleton(s =>
        {
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<IConsumer<TKey, TValue>>();
            var keyDeserializer = new KafkaDeserializer<TKey>(options.JsonSerializerOptions, loggerFactory.CreateLogger<KafkaDeserializer<TKey>>());
            var valueDeserializer = new KafkaDeserializer<TValue>(options.JsonSerializerOptions, loggerFactory.CreateLogger<KafkaDeserializer<TValue>>());

            var consumer = new ConsumerBuilder<TKey, TValue>(options)
                    .SetKeyDeserializer(keyDeserializer)
                    .SetValueDeserializer(valueDeserializer)
                    .SetErrorHandler((c, e) => options.ErrorHandler(logger, c, e))
                    .Build();

            consumer.Subscribe(options.Topic);
            
            return consumer;
        });
        services.AddHostedService<ConsumerService<TKey, TValue>>();
        services.AddScoped<IConsumerHandler<TKey, TValue>, THandler>();
    }

    /// <summary>
    /// Configures all sub Features
    /// </summary>
    /// <param name="features"></param>
    public void Configure(IFeatureCollection features)
    {
        features.Configure(Options);
    }
}
