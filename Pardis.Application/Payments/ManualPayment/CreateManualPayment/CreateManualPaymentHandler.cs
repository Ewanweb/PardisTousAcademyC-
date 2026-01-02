using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using Pardis.Domain.Courses;
using AutoMapper;

namespace Pardis.Application.Payments.ManualPayment.CreateManualPayment;

/// <summary>
/// Handler برای ایجاد درخواست پرداخت دستی
/// </summary>
public class CreateManualPaymentHandler : IRequestHandler<CreateManualPaymentCommand, OperationResult<ManualPaymentRequestDto>>
{
    private readonly IManualPaymentRequestRepository _manualPaymentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public CreateManualPaymentHandler(
        IManualPaymentRequestRepository manualPaymentRepository,
        ICourseRepository courseRepository,
        ICourseEnrollmentRepository enrollmentRepository,
        IMapper mapper)
    {
        _manualPaymentRepository = manualPaymentRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<ManualPaymentRequestDto>> Handle(CreateManualPaymentCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود دوره
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            return OperationResult<ManualPaymentRequestDto>.Error("دوره یافت نشد");

        // بررسی دوره رایگان
        if (course.Price == 0)
        {
            // برای دوره رایگان، مستقیماً ثبت‌نام ایجاد می‌کنیم
            var existingEnrollment = await _enrollmentRepository.GetEnrollmentAsync(request.StudentId, request.CourseId, cancellationToken);
            if (existingEnrollment != null)
                return OperationResult<ManualPaymentRequestDto>.Error("شما قبلاً در این دوره ثبت‌نام کرده‌اید");

            var freeEnrollment = new CourseEnrollment(request.CourseId, request.StudentId, 0);
            await _enrollmentRepository.AddAsync(freeEnrollment);
            await _enrollmentRepository.SaveChangesAsync(cancellationToken);

            return OperationResult<ManualPaymentRequestDto>.Success(new ManualPaymentRequestDto());
        }

        // بررسی وجود درخواست پرداخت دستی قبلی در حال انتظار
        var existingRequest = await _manualPaymentRepository.GetPendingRequestAsync(request.CourseId, request.StudentId, cancellationToken);
        if (existingRequest != null)
            return OperationResult<ManualPaymentRequestDto>.Error("درخواست پرداخت دستی قبلی برای این دوره در حال بررسی است");

        // بررسی عدم ثبت‌نام قبلی
        var existingCourseEnrollment = await _enrollmentRepository.GetEnrollmentAsync(request.StudentId, request.CourseId, cancellationToken);
        if (existingCourseEnrollment != null)
            return OperationResult<ManualPaymentRequestDto>.Error("شما قبلاً در این دوره ثبت‌نام کرده‌اید");

        // ایجاد درخواست پرداخت دستی
        var manualPaymentRequest = new ManualPaymentRequest(request.CourseId, request.StudentId, request.Amount);
        await _manualPaymentRepository.AddAsync(manualPaymentRequest);
        await _manualPaymentRepository.SaveChangesAsync(cancellationToken);

        // تبدیل به DTO با استفاده از AutoMapper
        var dto = _mapper.Map<ManualPaymentRequestDto>(manualPaymentRequest);
        dto.CourseName = course.Title; // اضافه کردن نام دوره

        return OperationResult<ManualPaymentRequestDto>.Success(dto);
    }
}