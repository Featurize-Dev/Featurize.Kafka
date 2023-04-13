using System.Collections;

namespace Kafka;

/// <summary>
/// Contains the information to register a producer.
/// </summary>
/// <param name="KeyType">The type of the key of the topic to produce.</param>
/// <param name="ValueType">The type of the event in the topic.</param>
/// <param name="Options">The options specific to this producer.</param>
public record ProducerInfo(Type KeyType, Type ValueType, ProducerOptions Options);

/// <summary>
/// Descibes a ProducerCollection
/// </summary>
public interface IProducerCollection : IReadOnlyCollection<ProducerInfo>
{
    /// <summary>
    /// Adds a ProducerInfo to the collection.
    /// </summary>
    /// <param name="producerInfo"></param>
    public void Add(ProducerInfo producerInfo);
}

/// <summary>
/// A default ProducerCollection
/// </summary>
public class ProducerCollection : IProducerCollection
{
    private readonly HashSet<ProducerInfo> _items = new();
    /// <inheritdoc />
    public int Count => _items.Count;
    /// <inheritdoc />
    public void Add(ProducerInfo producerInfo) => _items.Add(producerInfo);
    /// <inheritdoc />
    public IEnumerator<ProducerInfo> GetEnumerator() => _items.GetEnumerator();
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Extends the ProducerCollection
/// </summary>
public static class ProducerCollectionExtensions
{
    /// <summary>
    /// Adds a new producerinfo to the collection
    /// </summary>
    /// <typeparam name="TKey">The type of the Key.</typeparam>
    /// <typeparam name="TValue">The type of the Value.</typeparam>
    /// <param name="collection">The collection to add the producerinfo to.</param>
    /// <param name="options">The options for this producer.</param>
    public static void Add<TKey, TValue>(this IProducerCollection collection, ProducerOptions options)
    {
        collection.Add(new(typeof(TKey), typeof(TValue), options));
    }
}
