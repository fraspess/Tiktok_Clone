using MediatR;

namespace Application.Features.AdminPanel.MarkReportAsResolved;

public record MarkReportAsResolvedCommand(Guid Id) : IRequest<Unit>;