using Application.Dtos.Conversation;
using Application.Pagination;
using MediatR;

namespace Application.Features.Conversation.FindByUsername;

public record FindConversationByUsernameCommand(string query, PaginationSettings PaginationSettings) : IRequest<PagedResult<ConversationDto>>;
