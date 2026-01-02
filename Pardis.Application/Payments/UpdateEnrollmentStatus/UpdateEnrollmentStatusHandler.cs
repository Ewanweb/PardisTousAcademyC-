using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;

namespace Pardis.Application.Payments.UpdateEnrollmentStatus;

/// <summary>
/// Handler برای به‌روزرسانی وضعیت ثبت‌نام
/// </summary>
public class UpdateEnrollmentStatusHandler : IRequestHandler<UpdateEnrollmentStatusCommand, CourseEnrollmentDto>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public UpdateEnrollmentStatusHandler(ICourseEnrollmentRepository enrollmentRepository, IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<CourseEnrollmentDto> Handle(UpdateEnrollmentStatusCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود ثبت‌نام
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId);
        if (enrollment == null)
            throw new ArgumentException("ثبت‌نام یافت نشد", nameof(request.EnrollmentId));

        // به‌روزرسانی وضعیت
        switch (request.Status)
        {
            case EnrollmentStatus.Completed:
                enrollment.CompleteEnrollment();
                break;
            case EnrollmentStatus.Cancelled:
                enrollment.CancelEnrollment(request.Reason ?? "لغو شده توسط ادمین");
                break;
            case EnrollmentStatus.Suspended:
                // برای تعلیق، فعلاً فقط یادداشت اضافه می‌کنیم
                // در آینده می‌توان متد مخصوص تعلیق اضافه کرد
                break;
            case EnrollmentStatus.Active:
                // برای فعال کردن مجدد، فعلاً کاری نمی‌کنیم
                // در آینده می‌توان متد مخصوص فعال‌سازی اضافه کرد
                break;
        }

        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        // استفاده از AutoMapper برای تبدیل به DTO
        return _mapper.Map<CourseEnrollmentDto>(enrollment);
    }
}