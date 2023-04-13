using Confluent.Kafka;
using Featurize.Kafka;
using FluentAssertions;
using Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaFeature_Tests;
internal class Create
{
    [Test]
    public void Should_set_options_field()
    {
        var options = new KafkaOptions();
        var feature = KafkaFeature.Create(options);

        feature.Options.Should().Be(options);
    }
}

internal class Configure
{
    [Test]
    public void should_register_producer_services()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();

        var options = new KafkaOptions();

        options.AddProducer<Guid, string>();

        var feature = KafkaFeature.Create(options);

        feature.Configure(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();

        var kafkaProducer = () => provider.GetRequiredService<IProducer<Guid, string>>();
        var producer = () => provider.GetRequiredService<Producer<Guid, string>>();

        kafkaProducer.Should().NotThrow<InvalidOperationException>();
        producer.Should().NotThrow<InvalidOperationException>();
    }

    [Test]
    public void should_register_consumer_services()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();

        var options = new KafkaOptions();

        options.AddConsumer<TestHandler>();

        var feature = KafkaFeature.Create(options);

        feature.Configure(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();

        var kafkaConsumer = () => provider.GetRequiredService<IConsumer<Guid, string>>();
        var consumer = () => provider.GetRequiredService<IHostedService>() as ConsumerService<Guid, string>;
        var handler = () => provider.GetRequiredService<IConsumerHandler<Guid, string>>();


        kafkaConsumer.Should().NotThrow<InvalidOperationException>();
        consumer.Should().NotThrow<InvalidOperationException>();
        handler.Should().NotThrow<InvalidOperationException>();
    }
}

public class TestHandler : IConsumerHandler<Guid, string>
{
    public Task Handle(ConsumeResult<Guid, string> result)
    {
        throw new NotImplementedException();
    }
}
