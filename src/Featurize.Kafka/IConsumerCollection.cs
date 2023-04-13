using System.Collections;

namespace Kafka;

/// <summary>
/// Contains information to consruct a Consumer.
/// </summary>
/// <param name="HandlerType">The handler type that handles the message when consuming.</param>
/// <param name="KeyType">The type of the key of the topic.</param>
/// <param name="ValueType">The type of the events in the topic.</param>
/// <param name="Options">The options specific to this consumer.</param>
public record ConsumerInfo(Type HandlerType, Type KeyType, Type ValueType, ConsumerOptions Options);

/// <summary>
/// Descibes a consumer Collection.
/// </summary>
public interface IConsumerCollection : IReadOnlyCollection<ConsumerInfo>
{
    /// <summary>
    /// Adds a ConsumerInfo to the collection
    /// </summary>
    /// <param name="consumerInfo"></param>
    void Add(ConsumerInfo consumerInfo);
}

/// <summary>
/// Extends the functionality of a Consumer Collection
/// </summary>
public static class ConsumerCollectionExtensions
{
    /// <summary>
    /// Adds a Consumer to the collection
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="collection"></param>
    /// <param name="options"></param>
    public static void Add<THandler, TKey, TValue>(this IConsumerCollection collection, ConsumerOptions options)
        where THandler : IConsumerHandler<TKey, TValue>
        => collection.Add(new(typeof(THandler), typeof(TKey), typeof(TValue), options));

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="consumerCollection"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IConsumerCollection Add<THandler>(this IConsumerCollection consumerCollection, ConsumerOptions options)
    {
        var handlerType = typeof(THandler);
        var consumerType = handlerType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumerHandler<,>));

        if (consumerType is null)
        {
            throw new ArgumentException($"Consumer {handlerType} must implement {typeof(IConsumerHandler<,>)}");
        }

        var arguments = consumerType.GetGenericArguments();
        var keyType = arguments[0];
        var valueType = arguments[1];

        consumerCollection.Add(new ConsumerInfo(keyType, valueType, handlerType, options));
        return consumerCollection;
    }
}

/// <summary>
/// Default ConsumerCollection
/// </summary>
public class ConsumerCollection : IConsumerCollection
{
    private readonly HashSet<ConsumerInfo> _items = new();
    /// <inheritdoc />
    public int Count => _items.Count;
    /// <inheritdoc />
    public void Add(ConsumerInfo consumerInfo)
    {
        ArgumentNullException.ThrowIfNull(consumerInfo, nameof(consumerInfo));
        _items.Add(consumerInfo);
    }
    /// <inheritdoc />
    public IEnumerator<ConsumerInfo> GetEnumerator() => _items.GetEnumerator();
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
