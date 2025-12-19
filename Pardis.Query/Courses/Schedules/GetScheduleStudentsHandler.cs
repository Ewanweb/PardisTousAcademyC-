using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.Schedules;

public class GetScheduleStudentsHandler : IRequestHandler<GetScheduleStudentsQuery, List<ScheduleStudentDto>>
{
    private readonly IRepository<UserCourseSchedule> _userScheduleRepository;

    public GetScheduleStudentsHandler(IRepository<UserCourseSchedule> userScheduleRepository)
    {
        _userScheduleRepository = userScheduleRepository;
    }

    public async Task<List<ScheduleStudentDto>> Handle(GetScheduleStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _userScheduleRepository.Table
            .Include(ucs => ucs.User)
            .Include(ucs => ucs.CourseSchedule)
            .Where(ucs => ucs.CourseScheduleId == request.CourseScheduleId)
            .OrderBy(ucs => ucs.EnrolledAt)
            .Select(ucs => new ScheduleStudentDto
            {
                UserId = ucs.UserId,
                FullName = ucs.User.FullName ?? ucs.User.UserName ?? "",
                Email = ucs.User.Email ?? "",
                Mobile = ucs.User.PhoneNumber,
                EnrollmentDate = ucs.EnrolledAt,
                IsActive = ucs.Status == StudentScheduleStatus.Active,
                EnrolledAt = ucs.EnrolledAt,
                Status = ucs.Status.ToString(),
                AttendedSessions = ucs.AttendedSessions,
                AbsentSessions = ucs.AbsentSessions,
                InstructorNotes = ucs.InstructorNotes
            })
            .ToListAsync(cancellationToken);

        return students;
    }
}