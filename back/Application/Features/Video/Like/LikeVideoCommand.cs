using MediatR;

namespace Application.Features.Video.Like;

public record LikeVideoCommand(string VideoId) : IRequest<Unit>;