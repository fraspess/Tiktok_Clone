using Domain;
using MediatR;

namespace Application.Features.AdminPanel.BanVideo;

public record BanVideoCommand(Guid VideoId, VideoReportReasons Reason) : IRequest<Unit>;