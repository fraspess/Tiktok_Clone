using Application.Dtos.Video;
using MediatR;

namespace Application.Features.Video.Upload
{
    public record UploadVideoCommand(string ContentType) : IRequest<object>;
}