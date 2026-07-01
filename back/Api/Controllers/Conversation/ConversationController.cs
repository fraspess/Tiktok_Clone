using Api.RateLimiting;
using Application;
using Application.Dtos.Conversation;
using Application.Dtos.Message;
using Application.Features.Conversation.Create;
using Application.Features.Conversation.Get;
using Application.Features.Conversation.GetAll;
using Application.Features.Conversation.GetMessages;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Conversation;

[Route("api/conversations")]
[ApiController]
public class ConversationController(IMediator _mediator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetConversations(int pageNumber = 1, int pageSize = 10)
    {
        var conversations = await _mediator.Send(new GetConversationsQuery(
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<PagedResult<ConversationDto>>.Success(conversations));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConversationById(Guid id)
    {
        var conversation = await _mediator.Send(new GetConversationQuery(id));
        return Ok(ApiResponse<object>.Success(conversation));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var conversation = await _mediator.Send(new CreateConversationCommand(dto.UserIds));
        return Ok(ApiResponse<ConversationDto>.Success(conversation));
    }

    [HttpGet("messages")]
    [Authorize]
    public async Task<IActionResult> GetMessages(Guid conversationId, int pageNumber = 1, int pageSize = 10)
    {
        var messages = await _mediator.Send(new GetConversationMessagesQuery(conversationId,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<PagedResult<MessageDto>>.Success(messages));
    }
}