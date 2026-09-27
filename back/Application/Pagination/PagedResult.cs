namespace Application.Pagination;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];

    public PaginationMetadata Metadata { get; set; } = new();
}