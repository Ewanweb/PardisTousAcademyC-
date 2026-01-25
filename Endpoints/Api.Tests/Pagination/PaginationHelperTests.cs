using Pardis.Application._Shared.Pagination;
using Xunit;

namespace Api.Tests.Pagination;

public class PaginationHelperTests
{
    [Fact]
    public void Normalize_ClampsPageAndPageSize()
    {
        var result = PaginationHelper.Normalize(new PaginationRequest
        {
            Page = 0,
            PageSize = 500
        });

        Assert.Equal(PaginationHelper.MinPage, result.Page);
        Assert.Equal(PaginationHelper.MaxPageSize, result.PageSize);
    }

    [Fact]
    public void ClampPage_UsesLastPageWhenOutOfRange()
    {
        var result = PaginationHelper.ClampPage(new PaginationRequest
        {
            Page = 10,
            PageSize = 10
        }, totalCount: 35);

        Assert.Equal(4, result.Page);
    }

    [Fact]
    public void Create_SetsPaginationFlags()
    {
        var result = PaginationHelper.Create(
            new List<int> { 1, 2 },
            new PaginationRequest { Page = 2, PageSize = 10 },
            totalCount: 25);

        Assert.True(result.HasPrev);
        Assert.True(result.HasNext);
        Assert.Equal(3, result.TotalPages);
    }
}
