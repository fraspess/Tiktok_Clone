using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.UnRepost;

public class UnRepostVideoCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser) : IRequestHandler<UnRepostVideoCommand, Unit>
{
    public async Task<Unit> Handle(UnRepostVideoCommand request, CancellationToken cancellationToken)
    {
        var exists = await appDbContext.VideoReposts
            .Where(v => v.UserId == currentUser.Id!.Value && v.VideoId == request.VideoId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (exists is null) return Unit.Value;
        
        appDbContext.VideoReposts.Remove(exists);
        await appDbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}