using Application.Dtos.Video;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Video.GetById
{
    public class GetVideoByIdQueryHandler(IUnitOfWork _uow, IMapper _mapper, IConfiguration config, ICurrentUser currentUser)
        : IRequestHandler<GetVideoByIdQuery, VideoDTO>
    {
        public async Task<VideoDTO> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Videos
                .GetAll()
                .ProjectTo<VideoDTO>(_mapper.ConfigurationProvider, new { currentUserId = currentUser.Id, backendUrl = config["Backend:Url"] })
                .FirstOrDefaultAsync(v => v.Id == request.Id) ?? throw new NotFoundException("Відео не знайдено");
        }
    }
}