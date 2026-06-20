using MediatR;

namespace Application.Features.Video.Upload.Dev;

public record UploadVideoCommandDev(string Url, string Key, Guid[] RandomUserIds, string VideoDescription)
    : IRequest<Unit>;