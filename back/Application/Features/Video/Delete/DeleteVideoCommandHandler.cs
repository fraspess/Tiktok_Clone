using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Delete
{
    public class DeleteVideoCommandHandler(IAppDbContext appDbContext, ICurrentUser user) : IRequestHandler<DeleteVideoCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
        {
            var video = await appDbContext.Videos.FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken: cancellationToken)
                        ?? throw new NotFoundException("Відео не знайдено");

            if (video.UserId != user.Id)
            {
                throw new NotAllowedException("Ви не маєте прав на цю дію");
            }

            appDbContext.Videos.Remove(video);
            await appDbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}