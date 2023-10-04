using Confluent.Kafka;
using Featurize;
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
public class KafkaFeature : 
    IFeatureWithOptions<KafkaFeature, KafkaOptions>,
    IFeatureWithConfigurableOptions<KafkaOptions>,
    IServiceCollectionFeature
{
    private KafkaFeature(KafkaOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Options to configure a feature
    /// </summary>
    public KafkaOptions Options { get; }

    /// <summary>
    /// Creates a instance of the Kafka Feature
    /// </summary>
    /// <param name="config">The configuration of new Kafka Feature</param>
    /// <returns>A Instance of the kafka feature.</returns>
    public static KafkaFeature Create(KafkaOptions config)
    {
        return new KafkaFeature(config);
    }

    /// <summary>
    /// Configures services used by the kafka messaging feature
    /// </summary>
    /// <param name="services"></param>
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton(Options.SerializerOptions);
        
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
        services.AddSingleton(typeof(ISerializer<TKey>), options.KeySerializer);
        services.AddSingleton(typeof(ISerializer<TValue>), options.ValueSerializer);

        services.AddSingleton(s =>
        {
            var logger = s.GetRequiredService<ILogger<ProducerBuilder<TKey, TValue>>>();
            var keySerializer = s.GetRequiredService<ISerializer<TKey>>();
            var valueSerializer = s.GetRequiredService<ISerializer<TValue>>();

            var producer = new ProducerBuilder<TKey, TValue>(options)
                        .SetKeySerializer(keySerializer)
                        .SetValueSerializer(valueSerializer)
                        .SetErrorHandler((p, e) => options.ErrorHandler(logger, p, e))
                        .SetLogHandler((p, m) => options.LogHandler(logger, p, m))
                        .Build();

            return producer;
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
        services.AddSingleton(typeof(IDeserializer<TKey>), options.KeyDeserializer); 
        services.AddSingleton(typeof(IDeserializer<TValue>), options.ValueDeserializer);

        services.AddSingleton(s =>
        {
            var logger = s.GetRequiredService<ILogger<ConsumerBuilder<TKey, TValue>>>();
            var keyDeserializer = s.GetRequiredService<IDeserializer<TKey>>();
            var valueDeserializer = s.GetRequiredService<IDeserializer<TValue>>();

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
