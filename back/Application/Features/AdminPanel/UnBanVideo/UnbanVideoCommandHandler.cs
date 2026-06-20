using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.UnBanVideo;

internal class UnbanVideoCommandHandler(IAppDbContext appDbContext) : IRequestHandler<UnbanVideoCommand, Unit>
{
    public async Task<Unit> Handle(UnbanVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await appDbContext
                        .Videos
                        .FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken)
                    ?? throw new NotFoundException("Відео не знайдено");

        video.Unban();

        appDbContext.Videos.Update(video);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}