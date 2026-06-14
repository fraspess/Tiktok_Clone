using MediatR;

namespace Application.Features.Video.Upload.CompleteUpload;

public record CompleteUploadVideoCommand(Guid VideoId, string Description) : IRequest<Unit>;