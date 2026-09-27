namespace Contracts;

public interface IEventBus<T>
{
    public Task PublishAsync(T message);
}