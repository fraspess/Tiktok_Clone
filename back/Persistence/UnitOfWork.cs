using Application.Interfaces;
using Persistence.Repositories;
using Persistence.Repositories.Comment;
using Persistence.Repositories.Conversation;
using Persistence.Repositories.Favorite;
using Persistence.Repositories.Follow;
using Persistence.Repositories.HashTag;
using Persistence.Repositories.Like;
using Persistence.Repositories.Message;
using Persistence.Repositories.Report;
using Persistence.Repositories.Video;

namespace Persistence
{
    public class UnitOfWork(AppDbContext context, IStorageService storageService) : IUnitOfWork
    {
        // Changed to concrete type to match the concrete implementation being instantiated.
        public ICommentRepository Comments { get; } = new CommentRepository(context);
        public IVideoRepository Videos { get; } = new VideoRepository(context);
        public ILikeRepository Likes { get; } = new LikeRepository(context);
        public IHashTagRepository HashTags { get; } = new HashTagRepository(context);
        public IFollowRepository Follows { get; } = new FollowRepository(context);
        public IFavoriteRepository Favorites { get; } = new FavoriteRepository(context);
        public IConversationRepository Conversations { get; } = new ConversationRepository(context);
        public IMessageRepository Messages { get; } = new MessageRepository(context);

        public IReportRepository Reports { get; } = new ReportRepository(context, storageService);

        public Task<int> SaveChangesAsync() => context.SaveChangesAsync();

        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}