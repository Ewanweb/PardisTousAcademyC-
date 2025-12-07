using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Categories;
using Pardis.Infrastructure.Repository;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Categories.GetCategoryById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResource>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public GetCategoryByIdHandler(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoryResource> Handle(GetCategoryByIdQuery request, CancellationToken token)
        {
            var category = await _repository.GetCategoryById(request.Id, token);

            if (category == null)
            {
                // در کنترلر باید این حالت هندل شود (NotFound)
                return null;
            }

            return _mapper.Map<CategoryResource>(category);
        }
    }
}
