using MediatR;

namespace Application.Features.Favorite.ToggleFavorite
{
    public record ToggleFavoriteCommand(Guid VideoId) : IRequest<Unit>;
}