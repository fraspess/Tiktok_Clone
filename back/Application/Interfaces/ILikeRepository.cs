using Domain.Entities.Like;

namespace Application.Interfaces
{
    public interface ILikeRepository : IGenericRepository<LikeEntity>
    {
        public Task<LikeEntity?> GetLikeByUserAndVideoIdAsync(Guid userId, Guid videoId);
    }
}