using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Application.Blog.Utils;
using Pardis.Application.Blog.Validation;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Tags.CreateTag;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, OperationResult<TagDto>>
{
    private readonly IRepository<Tag> _tagRepository;

    public CreateTagCommandHandler(IRepository<Tag> tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<OperationResult<TagDto>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateTagRequestValidator();
        var validation = await validator.ValidateAsync(request.Dto, cancellationToken);
        if (!validation.IsValid)
            return OperationResult<TagDto>.Error(string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));

        var slug = SlugHelper.Normalize(string.IsNullOrWhiteSpace(request.Dto.Slug) ? request.Dto.Title : request.Dto.Slug);
        if (string.IsNullOrWhiteSpace(slug))
            return OperationResult<TagDto>.Error("اسلاگ معتبر نیست");

        var exists = await _tagRepository.Table.AnyAsync(t => t.Slug == slug, cancellationToken);
        if (exists)
            return OperationResult<TagDto>.Error("اسلاگ تکراری است");

        var tag = new Tag
        {
            Title = request.Dto.Title.Trim(),
            Slug = slug,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _tagRepository.AddAsync(tag);
        await _tagRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<TagDto>.Success(new TagDto
        {
            Id = tag.Id,
            Title = tag.Title,
            Slug = tag.Slug
        });
    }
}
