using Domain.Constants;
using MediatR;

namespace Application.Features.User.Settings.ChangeMessagePrivacy;

public record ChangeMessagePrivacyCommand(MessagePrivacy newMessagePrivacy) : IRequest<Unit>;