using System.Collections;

namespace Kafka;

public record ConsumerInfo(Type HandlerType, Type KeyType, Type ValueType, ConsumerOptions Options);

public interface IConsumerCollection : IEnumerable<ConsumerInfo>
{
    int count { get; }
    void Add(ConsumerInfo consumerInfo);
}

public static class ConsumerCollectionExtensions
{
    public static void Add<THandler, TKey, TValue>(this IConsumerCollection collection, ConsumerOptions options)
        where THandler : IConsumerHandler<TKey, TValue>
        => collection.Add(new(typeof(THandler), typeof(TKey), typeof(TValue), options));
}


internal class ConsumerCollection : IConsumerCollection
{
    private readonly HashSet<ConsumerInfo> _items = new();
    public int count => _items.Count;

    public void Add(ConsumerInfo consumerInfo)
    {
        ArgumentNullException.ThrowIfNull(consumerInfo, nameof(consumerInfo));
        _items.Add(consumerInfo);
    }

    public IEnumerator<ConsumerInfo> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
