using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using Pardis.Domain.Courses;
using AutoMapper;

namespace Pardis.Application.Payments;

/// <summary>
/// Handler برای ایجاد ثبت‌نام جدید
/// </summary>
public class CreateEnrollmentHandler : IRequestHandler<CreateEnrollmentCommand, CourseEnrollmentDto>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public CreateEnrollmentHandler(
        ICourseEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<CourseEnrollmentDto> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود دوره
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new ArgumentException("دوره یافت نشد", nameof(request.CourseId));

        // بررسی عدم ثبت‌نام قبلی
        var existingEnrollment = await _enrollmentRepository.GetEnrollmentAsync(request.StudentId, request.CourseId, cancellationToken);
        if (existingEnrollment != null)
            throw new InvalidOperationException("دانشجو قبلاً در این دوره ثبت‌نام کرده است");

        // ایجاد ثبت‌نام جدید
        var enrollment = new CourseEnrollment(
            request.CourseId,
            request.StudentId,
            request.TotalAmount,
            request.IsInstallmentAllowed,
            request.InstallmentCount);

        await _enrollmentRepository.AddAsync(enrollment);
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        // بازگشت DTO با استفاده از AutoMapper
        var dto = _mapper.Map<CourseEnrollmentDto>(enrollment);
        dto.CourseName = course.Title; // اضافه کردن نام دوره

        return dto;
    }
}