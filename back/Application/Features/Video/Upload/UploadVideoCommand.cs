using Application.Dtos.Video;
using MediatR;

namespace Application.Features.Video.Upload
{
    public record UploadVideoCommand(CreateVideoDTO Dto) : IRequest<Unit>;
}