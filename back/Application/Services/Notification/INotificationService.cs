namespace Application.Services.Notification;

public interface INotificationService
{
    public void FlushPendingAsync(Guid userId);
}