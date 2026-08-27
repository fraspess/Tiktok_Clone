using MediatR;

namespace Application.Features.Video.UnRepost;

public record UnRepostVideoCommand(Guid VideoId) : IRequest<Unit>;