using Confluent.Kafka;

namespace Kafka;

/// <summary>
/// A handler for handleing consumed messages.
/// </summary>
/// <typeparam name="TKey">Type of the partition key.</typeparam>
/// <typeparam name="TValue">Type of the message consumed</typeparam>
public interface IConsumerHandler<TKey, TValue>
{
    /// <summary>
    /// Method called whan a message is received by the consumer
    /// </summary>
    /// <param name="result">The consume results</param>
    /// <returns>Returns a task.</returns>
    public Task Handle(ConsumeResult<TKey, TValue> result);
}
