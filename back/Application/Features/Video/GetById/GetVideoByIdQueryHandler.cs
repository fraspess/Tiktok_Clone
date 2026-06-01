using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.GetById
{
    public class GetVideoByIdQueryHandler(IUnitOfWork _uow, ICurrentUser currentUser, VideoMapper videoMapper)
        : IRequestHandler<GetVideoByIdQuery, VideoDto>
    {
        public async Task<VideoDto> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
        {
            var video = await _uow.Videos
                .GetAll()
                .ToProjectionDto(currentUser.Id)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken: cancellationToken) ?? throw new NotFoundException("Відео не знайдено");

            return videoMapper.ToDto(video);
        }
    }
}