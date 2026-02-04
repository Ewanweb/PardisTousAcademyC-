using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Seo;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Categories;
using DomainSeoService = Pardis.Domain.Seo.ISeoService;

namespace Pardis.Application.Seo
{
    // Commands
    public record UpdateCourseSeoCommand(Guid CourseId, SeoMetadata SeoData) : IRequest<OperationResult>;
    public record UpdateCategorySeoCommand(Guid CategoryId, SeoMetadata SeoData) : IRequest<OperationResult>;
    public record GenerateCourseSeoCommand(Guid CourseId) : IRequest<OperationResult<SeoMetadata>>;
    public record ValidateSeoCommand(SeoMetadata SeoData) : IRequest<OperationResult<SeoValidationResult>>;

    // DTOs
    public class SeoValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public SeoMetadata OptimizedSeo { get; set; } = new();
    }

    // Handlers
    public class UpdateCourseSeoHandler : IRequestHandler<UpdateCourseSeoCommand, OperationResult>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly DomainSeoService _seoService;

        public UpdateCourseSeoHandler(ICourseRepository courseRepository, DomainSeoService seoService)
        {
            _courseRepository = courseRepository;
            _seoService = seoService;
        }

        public async Task<OperationResult> Handle(UpdateCourseSeoCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return OperationResult.NotFound("Course not found");

            // Validate SEO data
            if (!_seoService.ValidateSeoData(request.SeoData, out var errors))
                return OperationResult.Error(string.Join(", ", errors));

            // Update course SEO
            course.UpdateSeo(request.SeoData);
            
            await _courseRepository.SaveChangesAsync(cancellationToken);
            return OperationResult.Success();
        }
    }

    public class UpdateCategorySeoHandler : IRequestHandler<UpdateCategorySeoCommand, OperationResult>
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly DomainSeoService _seoService;

        public UpdateCategorySeoHandler(IRepository<Category> categoryRepository, DomainSeoService seoService)
        {
            _categoryRepository = categoryRepository;
            _seoService = seoService;
        }

        public async Task<OperationResult> Handle(UpdateCategorySeoCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return OperationResult.NotFound("Category not found");

            // Validate SEO data
            if (!_seoService.ValidateSeoData(request.SeoData, out var errors))
                return OperationResult.Error(string.Join(", ", errors));

            // Update category SEO
            category.UpdateSeo(request.SeoData);
            
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return OperationResult.Success();
        }
    }

    public class GenerateCourseSeoHandler : IRequestHandler<GenerateCourseSeoCommand, OperationResult<SeoMetadata>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly DomainSeoService _seoService;

        public GenerateCourseSeoHandler(ICourseRepository courseRepository, DomainSeoService seoService)
        {
            _courseRepository = courseRepository;
            _seoService = seoService;
        }

        public async Task<OperationResult<SeoMetadata>> Handle(GenerateCourseSeoCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return OperationResult<SeoMetadata>.NotFound("Course not found");

            var seoData = _seoService.GenerateDefaultSeo(
                course.Title,
                course.Description,
                course.Slug,
                SeoType.Course
            );

            return OperationResult<SeoMetadata>.Success(seoData);
        }
    }

    public class ValidateSeoHandler : IRequestHandler<ValidateSeoCommand, OperationResult<SeoValidationResult>>
    {
        private readonly DomainSeoService _seoService;

        public ValidateSeoHandler(DomainSeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task<OperationResult<SeoValidationResult>> Handle(ValidateSeoCommand request, CancellationToken cancellationToken)
        {
            var result = new SeoValidationResult();
            
            // Validate current SEO data
            result.IsValid = _seoService.ValidateSeoData(request.SeoData, out var errors);
            result.Errors = errors;

            // Generate warnings for optimization opportunities
            var warnings = new List<string>();
            
            if (request.SeoData.MetaTitle?.Length < 30)
                warnings.Add("Title could be more descriptive (recommended 30-60 characters)");
            
            if (request.SeoData.MetaDescription?.Length < 120)
                warnings.Add("Description could be more detailed (recommended 120-160 characters)");
            
            if (string.IsNullOrEmpty(request.SeoData.CanonicalUrl))
                warnings.Add("Canonical URL should be specified");

            result.Warnings = warnings;

            // Generate optimized version
            result.OptimizedSeo = new SeoMetadata(
                metaTitle: request.SeoData.MetaTitle,
                metaDescription: _seoService.OptimizeMetaDescription(request.SeoData.MetaDescription ?? ""),
                canonicalUrl: request.SeoData.CanonicalUrl,
                noIndex: request.SeoData.NoIndex,
                noFollow: request.SeoData.NoFollow
            );

            return OperationResult<SeoValidationResult>.Success(result);
        }
    }
}
