namespace Pardis.Application._Shared.Pagination;

public class PagedResult<TItem>
{
    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrev { get; set; }
    public object? Stats { get; set; }
}
