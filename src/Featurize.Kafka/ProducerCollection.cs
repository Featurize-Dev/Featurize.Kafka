using System.Collections;

namespace Kafka;

public record ProducerInfo(Type KeyType, Type ValueType, ProducerOptions Options);
public interface IProducerCollection : IEnumerable<ProducerInfo>
{
    public int Count { get; }

    public void Add(ProducerInfo producerInfo);
}

public class ProducerCollection : IProducerCollection
{
    private readonly HashSet<ProducerInfo> _items = new();
    public int Count => _items.Count;

    public void Add(ProducerInfo producerInfo) => _items.Add(producerInfo);

    public IEnumerator<ProducerInfo> GetEnumerator() => _items.GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class ProducerCollectionExtensions
{
    public static void Add<TKey, TValue>(this IProducerCollection collection, ProducerOptions options)
    {
        collection.Add(new(typeof(TKey), typeof(TValue), options));
    }
}
