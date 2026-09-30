using Application.Dtos.User;
using Application.Services.Notification;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR.Hubs;

[Authorize]
public class NotificationHubClient(INotificationService service) : Hub<INotificationHubClient>
{
    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        service.FlushPendingAsync(Guid.Parse(userId));
        return base.OnConnectedAsync();
    }
}