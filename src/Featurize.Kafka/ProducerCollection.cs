using System.Collections;

namespace Kafka;

/// <summary>
/// Describes a Producer
/// </summary>
/// <param name="KeyType">The type of the partition key</param>
/// <param name="ValueType">The type of the message</param>
/// <param name="Options">The options of used for this producer.</param>
public record ProducerInfo(Type KeyType, Type ValueType, ProducerOptions Options);

/// <summary>
/// Collection of producer registrations
/// </summary>
public interface IProducerCollection : IEnumerable<ProducerInfo>
{
    /// <summary>
    /// Total number of producers registerd.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Adds a producer to the collection.
    /// </summary>
    /// <param name="producerInfo"></param>
    public void Add(ProducerInfo producerInfo);
}

internal class ProducerCollection : IProducerCollection
{
    private readonly HashSet<ProducerInfo> _items = new();
    public int Count => _items.Count;

    public void Add(ProducerInfo producerInfo) => _items.Add(producerInfo);

    public IEnumerator<ProducerInfo> GetEnumerator() => _items.GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Extension method For Easier adding a Producer to the collection
/// </summary>
public static class ProducerCollectionExtensions
{
    /// <summary>
    /// Adds a Producer to the collection
    /// </summary>
    /// <typeparam name="TKey">The type of the partition key.</typeparam>
    /// <typeparam name="TValue">The type of the message produced by this producer.</typeparam>
    /// <param name="collection">The collection to add this producer to.</param>
    /// <param name="options">The options used for this producer.</param>
    public static void Add<TKey, TValue>(this IProducerCollection collection, ProducerOptions options)
    {
        collection.Add(new(typeof(TKey), typeof(TValue), options));
    }
}
