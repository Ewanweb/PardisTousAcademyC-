using AutoMapper;
using MediatR;
using Pardis.Infrastructure.Repository;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Categories.GetCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryResource>>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public GetCategoriesHandler(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CategoryResource>> Handle(GetCategoriesQuery request, CancellationToken token)
        {
            var categories = await _repository.GetCategories();



            return _mapper.Map<List<CategoryResource>>(categories);
        }
    }
}
