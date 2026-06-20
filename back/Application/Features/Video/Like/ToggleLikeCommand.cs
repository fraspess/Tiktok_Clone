using MediatR;

namespace Application.Features.Video.Like;

public record ToggleLikeCommand(Guid VideoId) : IRequest<Unit>;