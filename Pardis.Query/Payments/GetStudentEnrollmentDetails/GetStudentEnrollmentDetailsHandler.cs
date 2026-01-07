using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using Pardis.Query.Payments.GetStudentEnrollment;

namespace Pardis.Query.Payments.GetPayments;

/// <summary>
/// Handler برای دریافت جزئیات یک ثبت‌نام خاص
/// </summary>
public class GetStudentEnrollmentDetailsHandler : IRequestHandler<GetStudentEnrollmentDetailsQuery, CourseEnrollmentDto>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public GetStudentEnrollmentDetailsHandler(
        ICourseEnrollmentRepository enrollmentRepository,
        IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<CourseEnrollmentDto> Handle(GetStudentEnrollmentDetailsQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId);
        
        if (enrollment == null)
            return null;

        // بررسی اینکه این ثبت‌نام متعلق به دانشجو یا ادمین باشد
        if (enrollment.StudentId != request.StudentId)
            return null;

        // لود کردن اقساط اگر لود نشده باشند
        // نکته: اگر ریپازیتوری لود خودکار ندارد، در اینجا باید لود شود.
        // بر اساس Context قبلی، GetByIdAsync باید شامل اقساط باشد یا ریپازیتوری متد خاصی داشته باشد.
        
        // اگر ریپازیتوری متد خاصی برای دانشجو دارد از آن استفاده می‌کنیم
        var enrollments = await _enrollmentRepository.GetEnrollmentsWithInstallmentsAsync(request.StudentId, cancellationToken);
        var targetEnrollment = enrollments.Find(e => e.Id == request.EnrollmentId);

        if (targetEnrollment == null)
            return null;

        return _mapper.Map<CourseEnrollmentDto>(targetEnrollment);
    }
}
