using Application.Dtos.Message;
using Application.Interfaces;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    internal class ChatNotifier(IHubContext<ChatHub> hubContext) : IChatNotifier
    {
        public async Task SendMessageAsync(Guid recipientId, MessageDto message)
        {
            await hubContext.Clients
                .User(recipientId.ToString())
                .SendAsync("ReceivedMessage", message);
        }

        public async Task SendPendingMessagesAsync(Guid recipientId, IEnumerable<MessageDto> messages)
        {
            await hubContext.Clients
                .User(recipientId.ToString())
                .SendAsync("ReceivedPendingMessages", messages);
        }
    }
}