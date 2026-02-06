using FluentValidation;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Validation;

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequestDto>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Excerpt).NotEmpty().MaximumLength(500);
        RuleFor(x => x.BlogCategoryId).NotEmpty();
    }
}

public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequestDto>
{
    public UpdatePostRequestValidator()
    {
        Include(new CreatePostRequestValidator());
    }
}

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequestDto>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(150);
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequestDto>
{
    public UpdateCategoryRequestValidator()
    {
        Include(new CreateCategoryRequestValidator());
    }
}

public class CreateTagRequestValidator : AbstractValidator<CreateTagRequestDto>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(80);
    }
}

public class UpdateTagRequestValidator : AbstractValidator<UpdateTagRequestDto>
{
    public UpdateTagRequestValidator()
    {
        Include(new CreateTagRequestValidator());
    }
}
