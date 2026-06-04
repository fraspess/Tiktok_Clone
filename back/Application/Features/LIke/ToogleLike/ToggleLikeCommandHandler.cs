using Application.Interfaces;
using Domain.Entities.Like;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.LIke.ToogleLike
{
    public class ToggleLikeCommandHandler(ICurrentUser user, IAppDbContext appDbContext) : IRequestHandler<ToogleLikeCommand, Unit>
    {
        // Якщо є лайк забирає, нема - ставить
        public async Task<Unit> Handle(ToogleLikeCommand request, CancellationToken cancellationToken)
        {
            var video = await appDbContext.Videos.AnyAsync(v => v.Id  == request.VideoId, cancellationToken: cancellationToken);
            if (!video) throw new NotFoundException("Відео не знайдено");

            var existingLike = await appDbContext.Likes.FirstOrDefaultAsync(l => l.UserId == user.Id && l.VideoId == request.VideoId, cancellationToken: cancellationToken);

            if (existingLike is null)
            {
                await appDbContext.Likes.AddAsync(new LikeEntity
                {
                    UserId = user.Id!.Value,
                    VideoId = request.VideoId
                }, cancellationToken);
            }
            else
            {
               appDbContext.Likes.Remove(existingLike);
            }

            await appDbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}