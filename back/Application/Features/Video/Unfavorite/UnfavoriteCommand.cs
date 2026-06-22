using MediatR;

namespace Application.Features.Video.Unfavorite;

public record UnfavoriteCommand(string VideoId) : IRequest<Unit>;