using Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query,
            PaginationSettings paginationSettings, CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken: cancellationToken);
            var data = await query
                .Skip((paginationSettings.PageNumber - 1) * paginationSettings.PageSize)
                .Take(paginationSettings.PageSize)
                .ToListAsync(cancellationToken: cancellationToken);


            var totalPages = (int)Math.Ceiling(totalCount / (double)paginationSettings.PageSize);

            var pagedResult = new PagedResult<T>
            {
                Items = data,
                Metadata = new PaginationMetadata
                {
                    CurrentPage = paginationSettings.PageNumber,
                    PageSize = paginationSettings.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasNext = paginationSettings.PageNumber < totalPages,
                    HasPrevious = paginationSettings.PageNumber > 1
                }
            };

            return pagedResult;
        }
        
        public static PagedResult<TDestination> MapItems<TSource, TDestination>(
            this PagedResult<TSource> source,
            Func<TSource, TDestination> mapper)
        {
            return new PagedResult<TDestination>
            {
                Items = source.Items.Select(mapper).ToList(),
                Metadata = source.Metadata
            };
        }
    }
    
    
}