using Contracts;
using MassTransit;

namespace Infrastructure.RabbitMQ;

internal class EventBus<T>(IPublishEndpoint publishEndpoint) : IEventBus<T>
    where T : notnull
{
    public async Task PublishAsync(T message)
    {
        await publishEndpoint.Publish(message);
    }
}