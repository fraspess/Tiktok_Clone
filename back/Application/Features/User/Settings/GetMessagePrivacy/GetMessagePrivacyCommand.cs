using Domain.Constants;
using MediatR;

namespace Application.Features.User.Settings.GetMessagePrivacy;

public record GetMessagePrivacyCommand : IRequest<MessagePrivacy>;