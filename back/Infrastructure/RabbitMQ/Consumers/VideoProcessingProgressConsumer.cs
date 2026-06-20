using Application.Features.Video.ProcessInfoInPercent;
using Contracts.Events;
using MassTransit;
using MediatR;

namespace Infrastructure.RabbitMQ.Consumers;

internal class VideoProcessingProgressConsumer(IMediator _mediator) : IConsumer<VideoProcessingProgressEvent>
{
    public async Task Consume(ConsumeContext<VideoProcessingProgressEvent> context)
    {
        await _mediator.Send(
            new VideoProcessInfoInPercentCommand(context.Message.VideoId, context.Message.Progress));
    }
}