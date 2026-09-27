using MediatR;

namespace Application.Features.Video.ProcessInfoInPercent;

public record VideoProcessInfoInPercentCommand(Guid VideoId, int Percentage) : IRequest<Unit>;