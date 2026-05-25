using MediatR;

namespace Application.Features.LIke.ToogleLike
{
    public record ToogleLikeCommand(Guid VideoId) : IRequest<Unit>;
}