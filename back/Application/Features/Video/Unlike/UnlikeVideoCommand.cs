using MediatR;

namespace Application.Features.Video.Unlike;

public record UnlikeVideoCommand(string VideoId) : IRequest<Unit>;