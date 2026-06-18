using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.GetById
{
    public class GetVideoByIdQueryHandler(IAppDbContext appDbContext, ICurrentUser currentUser, VideoMapper videoMapper)
        : IRequestHandler<GetVideoByIdQuery, VideoDto>
    {
        public async Task<VideoDto> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
        {
            var video = await appDbContext
                .Videos
                .ToProjectionDto(currentUser.Id)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken: cancellationToken) 
                        ?? throw new NotFoundException("Відео не знайдено");
            var dto = videoMapper.ToDto(video);
            return dto;
        }
    }
}