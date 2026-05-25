using Application.Interfaces;
using Domain.Entities.Like;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.LIke.ToogleLike
{
    public class ToggleLikeCommandHandler(IUnitOfWork _uow, ICurrentUser user) : IRequestHandler<ToogleLikeCommand, Unit>
    {
        // Якщо є лайк забирає, нема - ставить
        public async Task<Unit> Handle(ToogleLikeCommand request, CancellationToken cancellationToken)
        {
            var video = await _uow.Videos.GetByIdAsync(request.VideoId)
                        ?? throw new NotFoundException("Відео не знайдено");

            var existingLike = await _uow.Likes.GetLikeByUserAndVideoIdAsync(user.Id!.Value, request.VideoId);

            if (existingLike == null)
            {
                await _uow.Likes.CreateAsync(new LikeEntity
                {
                    UserId = user.Id!.Value,
                    VideoId = request.VideoId
                });
            }
            else
            {
                await _uow.Likes.DeleteAsync(existingLike);
            }

            await _uow.SaveChangesAsync();
            return Unit.Value;
        }
    }
}