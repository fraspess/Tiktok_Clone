using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.AdminPanel.BanVideo;

internal class BanVideoCommandHandler(IUnitOfWork _uow, ICurrentUser user) : IRequestHandler<BanVideoCommand, Unit>
{
    public async Task<Unit> Handle(BanVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await _uow.Videos
                        .GetByIdAsync(request.VideoId)
                    ?? throw new NotFoundException("Відео не знайдено");

        video.Ban(user.Id!.Value);

        await _uow.Videos.UpdateAsync(video);
        await _uow.SaveChangesAsync();
        return Unit.Value;
    }
}