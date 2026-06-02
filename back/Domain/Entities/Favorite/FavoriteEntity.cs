using Domain.Entities.Identity;
using Domain.Entities.Video;

namespace Domain.Entities.Favorite
{
    public class FavoriteEntity : AuditableEntity
    {
        public Guid VideoId { get; set; }
        public VideoEntity Video { get; set; } = null!;
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
    }
}