namespace Application.Pagination;

public class PaginationSettings
{
    private const int _maxPageSize = 20;
    private const int _minPageNumber = 1;
    private const int _minPageSize = 1;
    private int _pageSize = 5;
    private int _pageNumber;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = Math.Clamp(value, _minPageNumber, int.MaxValue);
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, _minPageSize, _maxPageSize);
    }
}