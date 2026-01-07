using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Payments;

/// <summary>
/// Handler برای دریافت اقساط دانشجو
/// </summary>
public class GetStudentEnrollmentsHandler : IRequestHandler<GetStudentEnrollmentsQuery, List<CourseEnrollmentDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public GetStudentEnrollmentsHandler(
        IUserRepository userRepository,
        ICourseEnrollmentRepository enrollmentRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<List<CourseEnrollmentDto>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId);
        if (student == null)
            return new List<CourseEnrollmentDto>();

        var enrollments = await _enrollmentRepository.GetEnrollmentsWithInstallmentsAsync(request.StudentId, cancellationToken);

        // استفاده از AutoMapper برای تبدیل به DTO
        return _mapper.Map<List<CourseEnrollmentDto>>(enrollments);
    }
}