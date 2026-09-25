using Application.Services.Message;
using Microsoft.AspNetCore.Authorization;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR.Hubs;

[Authorize]
public class ChatHub(IMessageService messageService) : Hub
{
    public async Task SendMessage(Guid conversationId, string content)
    {
        try
        {
            await messageService.SendAsync(Guid.Parse(Context.UserIdentifier!), conversationId, content);
        }
        catch (NotAllowedException ex) when (ex.Message == ErrorCodes.MessagesNotAccepted)
        {
            throw new HubException(ErrorCodes.MessagesNotAccepted);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        await messageService.FlushPendingAsync(Guid.Parse(userId));
        await base.OnConnectedAsync();
    }

    public async Task MarkAsDelivered(Guid messageId)
    {
        await messageService.MarkAsDeliveredAsync(Guid.Parse(Context.UserIdentifier!), messageId);
    }

    public async Task MarkAsRead(Guid messageId)
    {
        await messageService.MarkAsReadAsync(Guid.Parse(Context.UserIdentifier!), messageId);
    }
}
