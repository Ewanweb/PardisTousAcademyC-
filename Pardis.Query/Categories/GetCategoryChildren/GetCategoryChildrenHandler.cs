using AutoMapper;
using MediatR;
using Pardis.Infrastructure.Repository;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Categories.GetCategoryChildren
{
    public class GetCategoryChildrenHandler : IRequestHandler<GetCategoryChildrenQuery, CategoryChildrenDto>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public GetCategoryChildrenHandler(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoryChildrenDto> Handle(GetCategoryChildrenQuery request, CancellationToken token)
        {
            var parent = await _repository.GetByIdAsync(request.ParentId);

            if (parent == null)
            {
                throw new Exception("دسته‌بندی والد یافت نشد.");
            }

            // 1. دریافت فرزندان مستقیم
            var children = await _repository.GetChildrenWithCourseCountAsync(request.ParentId, token);

            // 3. مپ کردن و پر کردن CoursesCount
            var childrenResources = children.Select(c =>
            {
                var resource = _mapper.Map<CategoryResource>(c.Category);
                resource.CoursesCount = c.CoursesCount;
                return resource;
            }).ToList();


            return new CategoryChildrenDto
            {
                Parent = _mapper.Map<CategoryResource>(parent),
                Children = childrenResources
            };
        }
    }
}
