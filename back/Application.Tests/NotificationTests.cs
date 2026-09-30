using Application.Dtos.Notification;
using Application.Dtos.Comment;
using Application.Dtos.Message;
using Application.Features.Comment.Create;
using Application.Features.Video.Like;
using Application.Features.Video.Favorite;
using Application.Features.User.FollowUser;
using Application.Mapper;
using Application.Services.Message;
using Application.Dtos.User;
using Application.Features.Comment.Like;
using Application.Features.Notification;
using Application.Features.Video.Repost;
using Application.Interfaces;
using Application.Pagination;
using Application.Services.Notification;
using Domain;
using Domain.Constants;
using Domain.Entities.Comment;
using Domain.Entities.Conversation;
using Domain.Exceptions;
using Domain.Entities.Identity;
using Domain.Entities.Notification;
using Domain.Entities.Video;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Persistence;
using Xunit;

namespace Application.Tests;

public class NotificationTests : IDisposable
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly AppDbContext db;
    private readonly CurrentUser user = new();
    private readonly RecordingNotifier notifier = new();
    private readonly NotificationService service;

    public NotificationTests()
    {
        connection.Open();
        db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        db.Database.EnsureCreated();
        service = new NotificationService(db, notifier, new Storage(), NullLogger<NotificationService>.Instance);
    }

    [Fact]
    public async Task HistoryIsRecipientScopedOrderedPagedAndIncludesReadItems()
    {
        var now = DateTime.UtcNow;
        db.Notifications.AddRange(
            new NotificationEntity {RecipientId = user.Id!.Value, CreatedAt = now.AddMinutes(-2)},
            new NotificationEntity {RecipientId = user.Id.Value, CreatedAt = now, ReadAt = now},
            new NotificationEntity {RecipientId = Guid.NewGuid(), CreatedAt = now.AddMinutes(1)});
        await db.SaveChangesAsync();
        var handler = new GetNotificationCommandHandler(db, service, user);
        var page = await handler.Handle(new GetNotificationsCommand(new PaginationSettings {PageNumber = 1, PageSize = 1}), default);
        Assert.Equal(2, page.Metadata.TotalCount);
        Assert.True(page.Metadata.HasNext);
        Assert.NotNull(Assert.Single(page.Items).ReadAt);
        var second = await handler.Handle(new GetNotificationsCommand(new PaginationSettings {PageNumber = 2, PageSize = 1}), default);
        Assert.Null(Assert.Single(second.Items).ReadAt);
    }

    [Fact]
    public async Task MarkReadAndMarkAllCannotChangeAnotherUsersNotifications()
    {
        var own = service.Create(user.Id!.Value, Guid.NewGuid(), NotificationType.NewFollower)!;
        var other = service.Create(Guid.NewGuid(), Guid.NewGuid(), NotificationType.NewFollower)!;
        await db.SaveChangesAsync();
        var read = new MarkNotificationsReadHandler(db, user);
        var count = new GetUnreadNotificationCountHandler(db, user);
        Assert.Equal(1, await count.Handle(new(), default));
        await read.Handle(new(other.Id), default);
        Assert.Equal(1, await count.Handle(new(), default));
        await read.Handle(new(own.Id), default);
        await read.Handle(new(own.Id), default);
        Assert.Equal(0, await count.Handle(new(), default));
        service.Create(user.Id.Value, Guid.NewGuid(), NotificationType.NewFollower);
        await db.SaveChangesAsync();
        await read.Handle(new(), default);
        Assert.Equal(0, await count.Handle(new(), default));
        Assert.Null((await db.Notifications.AsNoTracking().SingleAsync(n => n.Id == other.Id)).ReadAt);
    }

    [Fact]
    public async Task SelfActionsAreSuppressedAndDeliveryFailurePreservesHistory()
    {
        Assert.Null(service.Create(user.Id!.Value, user.Id.Value, NotificationType.YourVideoLiked));
        Assert.Empty(db.ChangeTracker.Entries<NotificationEntity>());
        var notification = service.Create(user.Id.Value, Guid.NewGuid(), NotificationType.NewFollower);
        await db.SaveChangesAsync();
        notifier.Fail = true;
        await service.PublishAsync([notification]);
        Assert.Equal(1, await db.Notifications.CountAsync());
    }

    [Theory]
    [InlineData(NotificationType.YourVideoLiked)]
    [InlineData(NotificationType.YourVideoAddedToFavorites)]
    [InlineData(NotificationType.YourVideoReposted)]
    [InlineData(NotificationType.YourVideoCommented)]
    [InlineData(NotificationType.YourCommentLiked)]
    [InlineData(NotificationType.YourCommentReplied)]
    public async Task EnrichmentResolvesActorAndVideoForVideoAndCommentEvents(NotificationType type)
    {
        var (actor, video, comment) = await SeedContent();
        var resource = type is NotificationType.YourCommentLiked or NotificationType.YourCommentReplied ? comment.Id : video.Id;
        var notification = service.Create(Guid.NewGuid(), actor.Id, type, resource)!;
        await db.SaveChangesAsync();
        var dto = Assert.Single(await service.ToDtosAsync([notification]));
        Assert.Equal(actor.UserName, dto.Actor!.Username);
        Assert.Equal(video.ShortId, dto.VideoShortId);
        video.IsDeleted = true;
        await db.SaveChangesAsync();
        Assert.Null(Assert.Single(await service.ToDtosAsync([notification])).VideoShortId);
    }

    [Fact]
    public async Task RepeatedRepostAndCommentUnlikeDoNotSendExtraNotifications()
    {
        var (_, video, comment) = await SeedContent();
        var repost = new RepostVideoCommandHandler(db, user, service);
        await repost.Handle(new RepostVideoCommand(video.Id), default);
        await repost.Handle(new RepostVideoCommand(video.Id), default);
        IRequestHandler<LikeCommentCommand, Unit> likes = new LikeCommentCommandHandler(db, user, service);
        await likes.Handle(new(comment.Id), default);
        await likes.Handle(new(comment.Id), default);
        Assert.Equal(2, await db.Notifications.CountAsync());
        Assert.Equal(2, notifier.Items.Count);
        Assert.Empty(await db.CommentLikes.ToListAsync());
    }

    [Fact]
    public async Task LikeFavoriteFollowCommentAndReplyCreateExpectedEvents()
    {
        var (actor, video, comment) = await SeedContent();
        using var provider = CreateProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new LikeVideoCommand(video.ShortId));
        await mediator.Send(new LikeVideoCommand(video.ShortId));
        await mediator.Send(new FavoriteVideoCommand(video.ShortId));
        await mediator.Send(new FavoriteVideoCommand(video.ShortId));
        await mediator.Send(new FollowUserCommand(actor.Id));
        await mediator.Send(new FollowUserCommand(actor.Id));
        await mediator.Send(new CreateCommentCommand(new CreateCommentDto {VideoId = video.ShortId, Text = "Comment"}));
        await mediator.Send(new CreateCommentCommand(new CreateCommentDto {VideoId = video.ShortId, Text = "Reply", ParentCommentId = comment.Id}));
        Assert.Equal(new[] {NotificationType.YourVideoLiked, NotificationType.YourVideoAddedToFavorites,
            NotificationType.NewFollower, NotificationType.YourVideoCommented, NotificationType.YourCommentReplied},
            notifier.Items.Select(n => n.Type));
        Assert.All(notifier.Items, n => Assert.Equal(actor.Id, n.RecipientId));
    }

    [Fact]
    public async Task MessageNotifiesOnlyRecipientAndReadingItClearsItsNotification()
    {
        var (recipient, _, _) = await SeedContent();
        using var provider = CreateProvider();
        var conversation = new ConversationEntity
        {
            Participants = [new ConversationParticipant {UserId = user.Id!.Value}, new ConversationParticipant {UserId = recipient.Id}]
        };
        db.Conversations.Add(conversation);
        await db.SaveChangesAsync();
        var messages = new MessageService(db, new MessageMapper(new Storage()), new ChatNotifier(),
            provider.GetRequiredService<UserManager<UserEntity>>(), new Storage(), user, new MessagePrivacyService(db), service);
        await messages.SendAsync(user.Id.Value, conversation.Id, "Hello");
        var notification = Assert.Single(notifier.Items);
        Assert.Equal(NotificationType.NewDMMessage, notification.Type);
        Assert.Equal(recipient.Id, notification.RecipientId);
        Assert.Equal(conversation.Id, notification.ConversationId);
        Assert.Equal((await db.Messages.SingleAsync()).Id, notification.ResourceId);
        await messages.MarkAsReadAsync(recipient.Id, notification.ResourceId!.Value);
        Assert.NotNull((await db.Notifications.AsNoTracking().SingleAsync()).ReadAt);
        recipient.MessagePrivacy = MessagePrivacy.Nobody;
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<NotAllowedException>(() => messages.SendAsync(user.Id.Value, conversation.Id, "Blocked"));
        Assert.Equal(1, await db.Notifications.CountAsync());
        Assert.Equal(1, await db.Messages.CountAsync());
    }

    private ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(db);
        services.AddSingleton<IAppDbContext>(db);
        services.AddSingleton<ICurrentUser>(user);
        services.AddSingleton<INotificationService>(service);
        services.AddIdentityCore<UserEntity>().AddEntityFrameworkStores<AppDbContext>();
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<GetNotificationsCommand>());
        return services.BuildServiceProvider();
    }

    private async Task<(UserEntity, VideoEntity, CommentEntity)> SeedContent()
    {
        var actor = new UserEntity {Id = Guid.NewGuid(), UserName = "creator", Email = "creator@example.test"};
        db.Users.AddRange(actor, new UserEntity {Id = user.Id!.Value, UserName = "viewer", Email = "viewer@example.test"});
        var video = new VideoEntity {Id = Guid.NewGuid(), UserId = actor.Id, ShortId = "test-video", Status = VideoStatus.Processed};
        var comment = new CommentEntity {Id = Guid.NewGuid(), UserId = actor.Id, VideoId = video.Id, Text = "Hello"};
        db.Videos.Add(video);
        db.Comments.Add(comment);
        await db.SaveChangesAsync();
        return (actor, video, comment);
    }

    public void Dispose() { db.Dispose(); connection.Dispose(); }

    private class CurrentUser : ICurrentUser
    {
        public Guid? Id { get; } = Guid.NewGuid();
        public bool IsAuthenticated => true;
    }

    private class RecordingNotifier : INotifier
    {
        public bool Fail { get; set; }
        public List<NotificationDto> Items { get; } = [];
        public Task SendNotificationAsync(NotificationDto dto)
        {
            if (Fail) throw new InvalidOperationException("Offline");
            Items.Add(dto);
            return Task.CompletedTask;
        }
    }

    private class Storage : IStorageService
    {
        public AvatarDto GetUserAvatar(Guid id) => new() {Small = "", Medium = "", Large = ""};
        public string GetVideoThumbnail(Guid id) => "";
        public string GetVideoEntryFile(Guid id) => "";
        public Task DeleteUserAvatars(Guid id) => Task.CompletedTask;
        public Task<string> GetVideoUploadPresignedUrlAsync(Guid id, string contentType) => Task.FromResult("");
    }

    private class ChatNotifier : IChatNotifier
    {
        public Task SendReceiptAsync(Guid recipientId, Guid conversationId, Guid messageId, bool isDelivered, bool isRead) => Task.CompletedTask;
        public Task SendMessageAsync(Guid recipientId, MessageDto message) => Task.CompletedTask;
        public Task SendPendingMessagesAsync(Guid recipientId, IEnumerable<MessageDto> messages) => Task.CompletedTask;
    }
}
