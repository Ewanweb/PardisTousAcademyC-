using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Application.Blog.Utils;
using Pardis.Application.Blog.Validation;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.Tags.UpdateTag;

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, OperationResult<TagDto>>
{
    private readonly IRepository<Tag> _tagRepository;

    public UpdateTagCommandHandler(IRepository<Tag> tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<OperationResult<TagDto>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateTagRequestValidator();
        var validation = await validator.ValidateAsync(request.Dto, cancellationToken);
        if (!validation.IsValid)
            return OperationResult<TagDto>.Error(string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));

        var tag = await _tagRepository.Table.FirstOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);
        if (tag == null)
            return OperationResult<TagDto>.NotFound("تگ یافت نشد");

        var slug = SlugHelper.Normalize(string.IsNullOrWhiteSpace(request.Dto.Slug) ? request.Dto.Title : request.Dto.Slug);
        if (string.IsNullOrWhiteSpace(slug))
            return OperationResult<TagDto>.Error("اسلاگ معتبر نیست");

        if (!string.Equals(tag.Slug, slug, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _tagRepository.Table.AnyAsync(t => t.Slug == slug && t.Id != tag.Id, cancellationToken);
            if (exists)
                return OperationResult<TagDto>.Error("اسلاگ تکراری است");

            tag.Slug = slug;
        }

        tag.Title = request.Dto.Title.Trim();
        tag.UpdatedAt = DateTime.UtcNow;

        _tagRepository.Update(tag);
        await _tagRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<TagDto>.Success(new TagDto
        {
            Id = tag.Id,
            Title = tag.Title,
            Slug = tag.Slug
        });
    }
}
