using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.CreateBlogCategory
{
    public record CreateBlogCategoryCommand(BlogCategoriesDTO Dto) : IRequest<OperationResult<BlogCategoriesResource>>;
}
