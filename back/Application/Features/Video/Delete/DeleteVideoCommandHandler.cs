using Application.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Delete;

public class DeleteVideoCommandHandler(IAppDbContext appDbContext, ICurrentUser user)
    : IRequestHandler<DeleteVideoCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await appDbContext.Videos.FirstOrDefaultAsync(v => v.ShortId == request.VideoId, cancellationToken)
                    ?? throw new NotFoundException(ErrorCodes.VideoNotFound);

        if (video.UserId != user.Id) throw new NotAllowedException(ErrorCodes.Forbidden);

        appDbContext.Videos.Remove(video);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}