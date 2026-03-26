public class PaginationFilter
{
    private const int MaxPageSize = 100;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public int Skip => (Page - 1) * PageSize;

    public void Normalize()
    {
        if (Page < 1) Page = 1;
        if (PageSize < 1) PageSize = 1;
        if (PageSize > MaxPageSize) PageSize = MaxPageSize;
    }
}
