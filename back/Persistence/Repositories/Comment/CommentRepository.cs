using Application.Interfaces;
using Domain.Entities.Comment;

namespace Persistence.Repositories.Comment
{
    internal class CommentRepository(AppDbContext _context) : GenericRepository<CommentEntity>(_context), ICommentRepository
    {

        public IQueryable<CommentEntity> GetCommentsByVideoId(Guid videoId)
        {
            return _context.Comments.Where(c => c.VideoId == videoId && c.ParentCommentId == null);
        }

        public IQueryable<CommentEntity> GetRepliesAsync(Guid commentId)
        {
            return _context.Comments.Where(c => c.ParentCommentId == commentId);
        }
    }
}