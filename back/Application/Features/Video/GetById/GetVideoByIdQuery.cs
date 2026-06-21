using Application.Dtos.Video;
using MediatR;

namespace Application.Features.Video.GetById;

public record GetVideoByIdQuery(string Id) : IRequest<VideoDto>;