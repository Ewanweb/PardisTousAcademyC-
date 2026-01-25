namespace Pardis.Application._Shared.Pagination;

public static class PaginationHelper
{
    public const int MinPage = 1;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 200;

    public static PaginationRequest Normalize(PaginationRequest request)
    {
        var page = request.Page < MinPage ? MinPage : request.Page;
        var pageSize = request.PageSize < MinPageSize ? MinPageSize : request.PageSize;
        pageSize = Math.Min(pageSize, MaxPageSize);

        return new PaginationRequest
        {
            Page = page,
            PageSize = pageSize
        };
    }

    public static PaginationRequest ClampPage(PaginationRequest request, int totalCount)
    {
        var totalPages = CalculateTotalPages(totalCount, request.PageSize);
        var page = request.Page;

        if (totalPages == 0)
        {
            page = MinPage;
        }
        else if (page > totalPages)
        {
            page = totalPages;
        }

        return new PaginationRequest
        {
            Page = page,
            PageSize = request.PageSize
        };
    }

    public static int CalculateTotalPages(int totalCount, int pageSize)
    {
        if (pageSize <= 0)
        {
            return 0;
        }

        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static PagedResult<TItem> Create<TItem>(
        IReadOnlyList<TItem> items,
        PaginationRequest request,
        int totalCount,
        object? stats = null)
    {
        var totalPages = CalculateTotalPages(totalCount, request.PageSize);
        var page = request.Page;

        return new PagedResult<TItem>
        {
            Items = items,
            Page = page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPrev = page > MinPage,
            HasNext = totalPages > 0 && page < totalPages,
            Stats = stats
        };
    }
}
