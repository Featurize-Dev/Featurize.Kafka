using ConsumerCollection_Tests;
using FluentAssertions;
using Kafka;
using System.Security.Cryptography;

namespace ProducerCollection_Tests;
internal class Count
{
    [Test]
    public void should_be_equal_to_number_of_elements()
    {
        var collection = new ProducerCollection();
        var rnd = RandomNumberGenerator.GetInt32(100);
        for (int i = 0; i < rnd; i++)
        {
            var item = new ProducerInfo(typeof(Guid), typeof(string), new ProducerOptions());
            collection.Add(item);
        }

        collection.Count().Should().Be(rnd);

    }
}
internal class Add
{
    [Test]
    public void should_add_item_to_collection()
    {
        var collection = new ProducerCollection();
        var item = new ProducerInfo(typeof(Guid), typeof(string), new ProducerOptions());
        collection.Add(item);

        collection.Should().HaveCount(1);
        collection.First().Should().Be(item);
    }

    [Test]
    public void should_not_add_same_item_twice()
    {
        var collection = new ProducerCollection();
        var item = new ProducerInfo(typeof(Guid), typeof(string), new ProducerOptions());

        collection.Add(item);
        collection.Add(item);

        collection.Should().HaveCount(1);
        collection.First().Should().Be(item);
    }


    [Test]
    public void should_add_if_options_differ()
    {
        var collection = new ProducerCollection();
        var item1 = new ProducerInfo(typeof(Guid), typeof(string), new ProducerOptions() { Topic = "test2" });
        var item2 = new ProducerInfo(typeof(Guid), typeof(string), new ProducerOptions() { Topic = "test1" });

        collection.Add(item1);
        collection.Add(item2);

        collection.Should().HaveCount(2);
    }
}
