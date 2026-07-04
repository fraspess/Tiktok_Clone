using Application.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.BanVideo;

internal class BanVideoCommandHandler(IAppDbContext appDbContext, ICurrentUser user)
    : IRequestHandler<BanVideoCommand, Unit>
{
    public async Task<Unit> Handle(BanVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await appDbContext
                        .Videos
                        .FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken)
                    ?? throw new NotFoundException(ErrorCodes.VideoNotFound);

        video.Ban(user.Id!.Value);

        appDbContext.Videos.Update(video);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}