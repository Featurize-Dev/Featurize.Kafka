using System.Collections;

namespace Kafka;

/// <summary>
/// Class to describe a consumer.
/// </summary>
/// <param name="HandlerType">The handle to call when message is reeceived.</param>
/// <param name="KeyType">The type of the partition key.</param>
/// <param name="ValueType">The type of the message consumed.</param>
/// <param name="Options">The options for this consumer</param>
public record ConsumerInfo(Type HandlerType, Type KeyType, Type ValueType, ConsumerOptions Options);

/// <summary>
/// A collection of consumers
/// </summary>
public interface IConsumerCollection : IEnumerable<ConsumerInfo>
{
    /// <summary>
    /// The number of consumers in this collection.
    /// </summary>
    int count { get; }

    /// <summary>
    /// Adds a consumer description to the collection.
    /// </summary>
    /// <param name="consumerInfo"></param>
    void Add(ConsumerInfo consumerInfo);
}

/// <summary>
/// Extension methods for easier adding consumers to the collection
/// </summary>
public static class ConsumerCollectionExtensions
{
    /// <summary>
    /// Adds a consumer to the collection.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler that is called when consuming a message.</typeparam>
    /// <typeparam name="TKey">The type of the partition key.</typeparam>
    /// <typeparam name="TValue">The type of the message.</typeparam>
    /// <param name="collection">The ConsumerCollection instance to add the consumer info to.</param>
    /// <param name="options">The options used for this consumer.</param>
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
