using MediatR;

namespace Application.Features.Video.ProcessFailed;

public record VideoProcessingFailedCommand(Guid VideoId, string Message) : IRequest<Unit>;