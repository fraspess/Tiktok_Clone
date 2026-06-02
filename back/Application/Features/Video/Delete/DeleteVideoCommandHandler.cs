using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Video.Delete
{
    public class DeleteVideoCommandHandler(IUnitOfWork _uow, ICurrentUser user) : IRequestHandler<DeleteVideoCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
        {
            var video = await _uow.Videos.GetByIdAsync(request.VideoId)
                        ?? throw new NotFoundException("Відео не знайдено");

            if (video.UserId != user.Id)
            {
                throw new NotAllowedException("Ви не маєте прав на цю дію");
            }

            await _uow.Videos.DeleteAsync(video);
            return Unit.Value;
        }
    }
}