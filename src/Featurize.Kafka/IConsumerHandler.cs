using Confluent.Kafka;

namespace Kafka;

public interface IConsumerHandler<TKey, TValue>
{
    public Task Handle(ConsumeResult<TKey, TValue> result);
}
