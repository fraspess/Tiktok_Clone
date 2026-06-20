using Application.Features.Video.ProcessFailed;
using Contracts.Events;
using MassTransit;
using MediatR;

namespace Infrastructure.RabbitMQ.Consumers;

internal class VideoProcessingFailedConsumer(IMediator _mediator) : IConsumer<VideoProcessingFailedEvent>
{
    public async Task Consume(ConsumeContext<VideoProcessingFailedEvent> context)
    {
        await _mediator.Send(new VideoProcessingFailedCommand(context.Message.Id, context.Message.Error));
    }
}