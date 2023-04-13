using Confluent.Kafka;
using FluentAssertions;
using Kafka;
using System.Security.Cryptography;

namespace ConsumerCollection_Tests;

internal class Count
{
    [Test]
    public void should_be_equal_to_number_of_elements()
    {
        var collection = new ConsumerCollection();
        var rnd = RandomNumberGenerator.GetInt32(100);
        for (int i = 0; i < rnd; i++)
        {
            var item = new ConsumerInfo(typeof(TestHandler), typeof(Guid), typeof(string), new ConsumerOptions());
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
        var collection = new ConsumerCollection();
        var item = new ConsumerInfo(typeof(TestHandler), typeof(Guid), typeof(string), new ConsumerOptions());
        collection.Add(item);

        collection.Should().HaveCount(1);
        collection.First().Should().Be(item);
    }

    [Test]
    public void should_not_add_same_item_twice()
    {
        var collection = new ConsumerCollection();
        var item = new ConsumerInfo(typeof(TestHandler), typeof(Guid), typeof(string), new ConsumerOptions());

        collection.Add(item);
        collection.Add(item);

        collection.Should().HaveCount(1);
        collection.First().Should().Be(item);
    }


    [Test]
    public void should_add_if_options_differ()
    {
        var collection = new ConsumerCollection();
        var item1 = new ConsumerInfo(typeof(TestHandler), typeof(Guid), typeof(string), new ConsumerOptions() { Topic = "test2" });
        var item2 = new ConsumerInfo(typeof(TestHandler), typeof(Guid), typeof(string), new ConsumerOptions() {  Topic = "test1" });

        collection.Add(item1);
        collection.Add(item2);

        collection.Should().HaveCount(2);
    }
}


internal class TestHandler : IConsumerHandler<Guid, string>
{
    public Task Handle(ConsumeResult<Guid, string> result)
    {
        throw new NotImplementedException();
    }
}
