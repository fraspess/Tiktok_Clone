using Application;
using Application.Dtos.Message;
using Application.Features.Message.Get;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Message
{
    [Route("api/messages")]
    [ApiController]
    public class MessageController(IMediator _mediator) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMessages(Guid conversationId, int pageNumber = 1, int pageSize = 10)
        {
            var messages = await _mediator.Send(new GetMessagesQuery(conversationId,
                new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
            return Ok(ApiResponse<PagedResult<MessageDto>>.Success(messages));
        }
    }
}