using MediatR;

namespace Application.Features.Video.Upload.CompleteUpload;

public record CompleteUploadVideoCommand(string Token, string Description) : IRequest<Unit>;