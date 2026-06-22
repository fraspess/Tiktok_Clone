using MediatR;

namespace Application.Features.Video.Favorite;

public record FavoriteVideoCommand(string VideoId) : IRequest<Unit>;