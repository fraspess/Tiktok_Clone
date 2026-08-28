using MediatR;

namespace Application.Features.Video.Repost;

public record RepostVideoCommand(Guid VideoId) : IRequest<Unit>;