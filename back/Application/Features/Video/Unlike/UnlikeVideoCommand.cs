using MediatR;

namespace Application.Features.Video.UnLike;

public record UnlikeVideoCommand(string VideoId) : IRequest<Unit>;