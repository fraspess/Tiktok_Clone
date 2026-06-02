using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;
using Domain.Entities.Video;

namespace Domain.Entities.Comment
{
    public class CommentEntity : BannableSoftDeletableEntity
    {
        [MaxLength()]
        public required String Text { get; set; }

        public required Guid UserId { get; set; }

        public UserEntity? Author { get; set; }

        public required Guid VideoId { get; set; }

        public VideoEntity? Video { get; set; }

        public Guid? ParentCommentId { get; set; }
        public CommentEntity? ParentComment { get; set; }

        public ICollection<CommentEntity> Replies { get; set; } = new List<CommentEntity>();

        public ICollection<CommentLikeEntity> CommentLikes { get; set; } = new List<CommentLikeEntity>();
    }
}