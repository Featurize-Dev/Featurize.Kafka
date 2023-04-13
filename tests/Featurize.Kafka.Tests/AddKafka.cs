using Featurize;
using FluentAssertions;
using Kafka;

namespace KafkaFeature_Tests;

public class AddKafka
{
    [Test]
    public void Should_add_feature()
    {
        var collection = new FeatureCollection();

        collection.AddKafka();

        collection.OfType<KafkaFeature>().Should().HaveCount(1);
    }
}