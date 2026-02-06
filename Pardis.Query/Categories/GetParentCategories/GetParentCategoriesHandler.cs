using AutoMapper;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Categories;
using Pardis.Infrastructure.Repository;

namespace Pardis.Query.Categories.GetParentCategories;

public class GetParentCategoriesHandler : IRequestHandler<GetParentCategoriesQuery, OperationResult<List<CategoryResource>>>
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public GetParentCategoriesHandler(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<OperationResult<List<CategoryResource>>> Handle(GetParentCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = _repository.Table.Where(x => x.ParentId == null).ToList();

            var resource = _mapper.Map<List<CategoryResource>>(result);

            return  OperationResult<List<CategoryResource>>.Success(resource);

        }
        catch (Exception e)
        {
            return new OperationResult<List<CategoryResource>>();
        }
    }
}