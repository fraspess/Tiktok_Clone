using Application.Dtos.Message;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entities.Message;
using Domain.Constants;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Message;

public class MessageService(IAppDbContext appDbContext,
    MessageMapper _mapper, 
    IChatNotifier _notifier,
    UserManager<UserEntity> userManager,
    IStorageService storageService,
    ICurrentUser currentUser,
    MessagePrivacyService messagePrivacy
    )
    : IMessageService
{
    public async Task FlushPendingAsync(Guid userId)
    {
        var pendingMessages = await appDbContext.Messages
            .Where(m =>
                m.IsDelivered == false &&
                m.SenderId != userId &&
                m.Conversation.Participants
                    .Any(p => p.UserId == userId))
            .OrderBy(m => m.CreatedAt)
            .Include(m => m.Sender)
            .ToListAsync();

        if (pendingMessages.Count == 0) return;


        var dtos = pendingMessages.Select(m => _mapper.ToDto(m)).ToList();
        await _notifier.SendPendingMessagesAsync(userId, dtos);
    }

    public async Task MarkAsDeliveredAsync(Guid userId, Guid messageId)
    {
        var message = await appDbContext
                          .Messages
                          .FirstOrDefaultAsync(m =>
                              m.Id == messageId && m.SenderId != userId &&
                              m.Conversation.Participants.Any(p => p.UserId == userId))
                      ?? throw new NotFoundException(ErrorCodes.MessageNotFound, "Message not found.");

        if (message.IsDelivered) return;
        message.IsDelivered = true;
        await appDbContext.SaveChangesAsync();
        await NotifyReceiptAsync(message);
    }

    public async Task MarkAsReadAsync(Guid userId, Guid messageId)
    {
        var message = await appDbContext.Messages
                          .FirstOrDefaultAsync(m =>
                              m.Id == messageId && m.SenderId != userId &&
                              m.Conversation.Participants.Any(p => p.UserId == userId))
                      ?? throw new NotFoundException(ErrorCodes.MessageNotFound, "Message not found.");

        if (message.IsRead) return;
        message.IsDelivered = true;
        message.IsRead = true;
        await appDbContext.SaveChangesAsync();
        await NotifyReceiptAsync(message);
    }

    private async Task NotifyReceiptAsync(MessageEntity message)
    {
        var recipients = await appDbContext.Conversations.Where(c => c.Id == message.ConversationId)
            .SelectMany(c => c.Participants.Select(p => p.UserId)).ToListAsync();
        foreach (var recipient in recipients)
            await _notifier.SendReceiptAsync(recipient, message.ConversationId, message.Id, message.IsDelivered, message.IsRead);
    }

    public async Task SendAsync(Guid userId, Guid conversationId, string content)
    {
        var conversationParticipants = await appDbContext.Conversations.Where(c => c.Id == conversationId)
            .Select(p => p.Participants).FirstOrDefaultAsync();
        if (conversationParticipants is null) throw new NotFoundException(ErrorCodes.ConversationNotFound, "Conversation not found.");
        if (conversationParticipants.All(p => p.UserId != currentUser.Id!.Value)) throw new NotAllowedException(ErrorCodes.Forbidden);

        foreach (var recipient in conversationParticipants.Where(p => p.UserId != userId))
        {
            await messagePrivacy.EnsureCanMessageAsync(userId, recipient.UserId);
        }
        
        var senderUsername = await userManager.Users.Where(u => u.Id == userId).Select(u => u.UserName).FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorCodes.UserNotFound);
        
        var newMessage = new MessageEntity
        {
            SenderId = userId,
            ConversationId = conversationId,
            Content = content
        };

        await appDbContext.Messages.AddAsync(newMessage);
        await appDbContext.SaveChangesAsync();

        var dto = new MessageDto
        {
            Id = newMessage.Id,
            ConversationId = conversationId,
            Content = newMessage.Content,
            SenderId = newMessage.SenderId,
            IsOwn = false,
            CreatedAt = newMessage.CreatedAt,
            SenderUsername = senderUsername,
            SenderAvatarUrl = storageService.GetUserAvatar(userId)
        };
        
        // Queue the sender's message before a recipient can acknowledge it.
        foreach(var participant in conversationParticipants.OrderByDescending(p => p.UserId == userId))
        {
            await _notifier.SendMessageAsync(participant.UserId, dto);
        }
    }
}
