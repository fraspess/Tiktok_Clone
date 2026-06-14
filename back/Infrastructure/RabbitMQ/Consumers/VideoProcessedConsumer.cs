using Application.Features.Video.Processed;
using Contracts.Events;
using MassTransit;
using MediatR;

namespace Infrastructure.RabbitMQ.Consumers
{
    internal class VideoProcessedConsumer(IMediator _mediator) : IConsumer<VideoProcessedEvent>
    {
        public async Task Consume(ConsumeContext<VideoProcessedEvent> context)
        {
            await _mediator.Send(new VideoProcessedCommand(context.Message.VideoId));
        }
    }
}