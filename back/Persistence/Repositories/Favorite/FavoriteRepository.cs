using Application.Interfaces;
using Domain.Entities.Favorite;

namespace Persistence.Repositories.Favorite
{
    internal class FavoriteRepository(AppDbContext _context) : GenericRepository<FavoriteEntity>(_context), IFavoriteRepository
    {

        public FavoriteEntity GetByVideoAndUserIds(Guid videoId, Guid userId)
        {
            return _context.Favorites.Where(f => f.VideoId == videoId && f.UserId == userId).FirstOrDefault()!;
        }
    }
}