using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application._Shared.Pagination;

public interface IPagedQuery<TItem> : IRequest<OperationResult<PagedResult<TItem>>>
{
    PaginationRequest Pagination { get; set; }
}

public abstract class PagedQuery<TItem> : IPagedQuery<TItem>
{
    public PaginationRequest Pagination { get; set; } = new();
}
