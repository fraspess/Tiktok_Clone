using Domain.Entities.Favorite;

namespace Application.Interfaces
{
    public interface IFavoriteRepository : IGenericRepository<FavoriteEntity>
    {
        public FavoriteEntity GetByVideoAndUserIds(Guid videoId, Guid userId);
    }
}