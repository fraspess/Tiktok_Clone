using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.AdminPanel.UnBanVideo;

internal class UnbanVideoCommandHandler(IUnitOfWork _uow) : IRequestHandler<UnbanVideoCommand, Unit>
{
    public async Task<Unit> Handle(UnbanVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await _uow.Videos
                        .GetByIdAsync(request.VideoId)
                    ?? throw new NotFoundException("Відео не знайдено");
        
        video.Unban();
        
        await _uow.Videos.UpdateAsync(video);
        await _uow.SaveChangesAsync();
        return Unit.Value;
    }
}