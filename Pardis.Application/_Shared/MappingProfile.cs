using Pardis.Domain.Dto.Seo;
using AutoMapper;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Seo;
using Pardis.Domain.Users;
using Pardis.Domain.Accounting;
using Pardis.Domain.Comments;
using Pardis.Domain.Attendance;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto;
using Pardis.Domain.Payments;
using Pardis.Domain.Sliders;
using Pardis.Domain.Shopping;
using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Dto.Accounting;
using Pardis.Domain.Dto.Comments;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Dto.Sliders;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application._Shared
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ????? SeoMetadata ?? SeoDto
            CreateMap<SeoMetadata, SeoDto>().ReverseMap();

            // Slider Mappings - Simplified structure
            CreateMap<HeroSlide, HeroSlideResource>()
                .ForMember(dest => dest.ActionLabel, opt => opt.MapFrom(src => src.ActionLabel))
                .ForMember(dest => dest.ActionLink, opt => opt.MapFrom(src => src.ActionLink));

            CreateMap<HeroSlide, HeroSlideListResource>()
                .ForMember(dest => dest.ActionLabel, opt => opt.MapFrom(src => src.ActionLabel))
                .ForMember(dest => dest.ActionLink, opt => opt.MapFrom(src => src.ActionLink));
                
            CreateMap<SuccessStory, SuccessStoryResource>()
                .ForMember(dest => dest.ActionLabel, opt => opt.MapFrom(src => src.ActionLabel))
                .ForMember(dest => dest.ActionLink, opt => opt.MapFrom(src => src.ActionLink));
                
            CreateMap<SuccessStory, SuccessStoryListResource>()
                .ForMember(dest => dest.ActionLabel, opt => opt.MapFrom(src => src.ActionLabel))
                .ForMember(dest => dest.ActionLink, opt => opt.MapFrom(src => src.ActionLink));

            // ????? User ?? UserResource (???? Courses ???? ??????? ?? circular reference)
            CreateMap<User, UserResource>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.AvatarFileId, opt => opt.MapFrom(src => src.AvatarFileId))
                .ForMember(dest => dest.AvatarUpdatedAt, opt => opt.MapFrom(src => src.AvatarUpdatedAt))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.NationalCode, opt => opt.MapFrom(src => src.NationalCode))
                .ForMember(dest => dest.FatherName, opt => opt.MapFrom(src => src.FatherName))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Roles ?? ??????? handle ??
                .ReverseMap();

            // ? User Profile Mappings
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? src.Mobile))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.AvatarUpdatedAt, opt => opt.MapFrom(src => src.AvatarUpdatedAt))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // ????? Category ?? CategoryResource
            CreateMap<Category, CategoryResource>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ReverseMap(); 

            CreateMap<CategoryWithCountDto, CategoryResource>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Category.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Category.Title))
                .ForMember(dest => dest.CoursesCount, opt => opt.MapFrom(src => src.CoursesCount))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Category.Slug))
                .ForMember(dest => dest.Seo, opt => opt.MapFrom(src => src.Category.Seo)).ReverseMap();
            // Ignore ???? Children ??? ?? ??? DTO ??? ????????

            // ????? Course ?? CourseResource
            CreateMap<CourseSection, CourseSectionDto>().ReverseMap();

            CreateMap<AuthLog, AuthLogDTO>()
                .ForMember(dest => dest.Ip, opt => opt.MapFrom(src => src.Ip))
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            // ????? CourseSchedule ?? CourseScheduleDto
            CreateMap<CourseSchedule, CourseScheduleDto>()
                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => src.GetDayName()))
                .ForMember(dest => dest.TimeRange, opt => opt.MapFrom(src => $"{src.StartTime:HH:mm}-{src.EndTime:HH:mm}"))
                .ForMember(dest => dest.FullScheduleText, opt => opt.MapFrom(src => src.GetFullScheduleText()))
                .ForMember(dest => dest.RemainingCapacity, opt => opt.MapFrom(src => src.RemainingCapacity))
                .ForMember(dest => dest.HasCapacity, opt => opt.MapFrom(src => src.HasCapacity));

            // ????? User ?? InstructorBasicDto (???? Courses)
            CreateMap<User, InstructorBasicDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile));

            CreateMap<UserCourse, CourseDetailDto>()
                // ???) ????? ???? ??????? ?? ?? "Course" ????? (Title, Price, Image, ...)
                .IncludeMembers(s => s.Course)
                
                // ?) ??? ??????? ??????? ?? ?? "UserCourse" ???????? ??
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.IsPurchased, opt => opt.MapFrom(src => true)) // ??? UserCourse ???? ?? ?????
                .ForMember(dest => dest.IsInCart, opt => opt.MapFrom(src => false)) // ??? ?????? ???? ?? ??? ????
                .ForMember(dest => dest.Access, opt => opt.MapFrom(src => new CourseAccessDto
                {
                    ClassLocation = GetClassLocationForCourseType(src.Course.Type, src.Course.Location),
                    LiveUrl = GetLiveUrlForCourseType(src.Course.Type, src.ExclusiveLiveLink),
                    IsAccessBlocked = src.IsAccessBlocked,
                    SeatNumber = src.SeatNumber,
                    EnrolledAt = src.EnrolledAt,
                    IsCompleted = src.IsCompleted,
                    CompletedAt = src.CompletedAt,
                    FinalGrade = src.FinalGrade,
                    CertificateCode = src.CertificateCode
                }))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Course.CreatedAt));

            CreateMap<UserCourse, CourseResource>()
                // ???) ????? ???? ??????? ?? ?? "Course" ????? (Title, Price, Image, ...)
                // ??? ??? ???? ?????? ?? ?????? "Course -> CourseResource" ??????? ???
                .IncludeMembers(s => s.Course)

                // ?) ??? ??????? ??????? ?? ?? "UserCourse" ???????? ??
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CourseId)) // ????? ???? ?? ????? ?? ????? ???? ????
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted)) // ????? ????? ??????
                .ForMember(dest => dest.IsStarted, opt => opt.MapFrom(src => true)) // ??? ??????? ????? ???? ???? ????
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Course.CreatedAt)); // ????? ????? ???? (?? ????? ???????)
            // ??? ??????? ????? ??????? ?? ?????????? ???? ?? ???? EnrolledAt ?? CourseResource ????? ????

            // ????? Course ?? CourseDetailDto (???? ??????? ?????????)
            CreateMap<Course, CourseDetailDto>()
                .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections))
                .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Instructor))
                .ForMember(dest => dest.IsPurchased, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsInCart, opt => opt.MapFrom(src => false)) // ???? ?? ??? ???? ?????? ???
                .ForMember(dest => dest.Access, opt => opt.MapFrom(src => (CourseAccessDto)null));

            // ????? Course ?? CourseResource
            CreateMap<Course, CourseResource>()
                .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections))
                .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Instructor))
                .ReverseMap();

            // ????? Transaction ?? TransactionDto
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.FullName ?? src.User.UserName ?? "??????"))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.User.Email ?? ""))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetTransactionStatusDisplay(src.Status)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
                .ForMember(dest => dest.MethodDisplay, opt => opt.MapFrom(src => GetPaymentMethodDisplay(src.Method)));

            // ? Comments Mappings
            CreateMap<CourseComment, CourseCommentDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.FullName ?? src.User.UserName ?? "??????"))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetCommentStatusDisplay(src.Status)))
                .ForMember(dest => dest.ReviewedByUserName, opt => opt.MapFrom(src => src.ReviewedByUser != null ? src.ReviewedByUser.FullName ?? src.ReviewedByUser.UserName : null));

            // ? Attendance Mappings
            CreateMap<CourseSession, CourseSessionDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetSessionStatusDisplay(src.Status)))
                .ForMember(dest => dest.TotalStudents, opt => opt.MapFrom(src => src.Attendances.Count))
                .ForMember(dest => dest.PresentStudents, opt => opt.MapFrom(src => src.Attendances.Count(a => a.Status == AttendanceStatus.Present)))
                .ForMember(dest => dest.AbsentStudents, opt => opt.MapFrom(src => src.Attendances.Count(a => a.Status == AttendanceStatus.Absent)))
                .ForMember(dest => dest.LateStudents, opt => opt.MapFrom(src => src.Attendances.Count(a => a.Status == AttendanceStatus.Late)));

            CreateMap<StudentAttendance, StudentAttendanceDto>()
                .ForMember(dest => dest.SessionTitle, opt => opt.MapFrom(src => src.Session.Title))
                .ForMember(dest => dest.SessionDate, opt => opt.MapFrom(src => src.Session.SessionDate))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName ?? src.Student.UserName ?? "??????"))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetAttendanceStatusDisplay(src.Status)))
                .ForMember(dest => dest.AttendanceDuration, opt => opt.MapFrom(src => src.GetAttendanceDuration()))
                .ForMember(dest => dest.RecordedByUserName, opt => opt.MapFrom(src => src.RecordedByUser != null ? src.RecordedByUser.FullName ?? src.RecordedByUser.UserName : null));

            // ? Payments Mappings
            CreateMap<CourseEnrollment, CourseEnrollmentDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : "??????"))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName ?? src.Student.UserName : "??????"))
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => src.GetRemainingAmount()))
                .ForMember(dest => dest.PaymentPercentage, opt => opt.MapFrom(src => src.GetPaymentPercentage()))
                .ForMember(dest => dest.PaymentStatusDisplay, opt => opt.MapFrom(src => GetPaymentStatusDisplay(src.PaymentStatus)))
                .ForMember(dest => dest.EnrollmentStatusDisplay, opt => opt.MapFrom(src => GetEnrollmentStatusDisplay(src.EnrollmentStatus)))
                .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.InstallmentPayments));

            CreateMap<InstallmentPayment, InstallmentPaymentDto>()
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => src.GetRemainingAmount()))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetInstallmentStatusDisplay(src.Status)))
                .ForMember(dest => dest.PaymentMethodDisplay, opt => opt.MapFrom(src => src.PaymentMethod.HasValue ? GetEnrollmentPaymentMethodDisplay(src.PaymentMethod.Value) : null))
                .ForMember(dest => dest.DaysUntilDue, opt => opt.MapFrom(src => src.GetDaysUntilDue()))
                .ForMember(dest => dest.DaysOverdue, opt => opt.MapFrom(src => src.GetDaysOverdue()));

            // PaymentAttempt to ManualPaymentRequestDto (simplified for manual payments only)
            CreateMap<PaymentAttempt, ManualPaymentRequestDto>()
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order != null ? src.Order.OrderNumber : "??????"))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName ?? src.User.UserName ?? src.User.Email : "??????"))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetPaymentAttemptStatusDisplay(src.Status)));

        }


        private static string GetPaymentStatusDisplay(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Unpaid => "?????? ????",
                PaymentStatus.Partial => "?????? ????",
                PaymentStatus.Paid => "?????? ????",
                _ => status.ToString()
            };
        }

        private static string GetPaymentAttemptStatusDisplay(PaymentAttemptStatus status)
        {
            return status switch
            {
                PaymentAttemptStatus.Draft => "????????",
                PaymentAttemptStatus.PendingPayment => "?? ?????? ????? ????",
                PaymentAttemptStatus.AwaitingAdminApproval => "?? ?????? ????? ?????",
                PaymentAttemptStatus.Paid => "????? ???",
                PaymentAttemptStatus.Failed => "?? ???",
                _ => status.ToString()
            };
        }
        

        private static string GetTransactionStatusDisplay(TransactionStatus status)
        {
            return status switch
            {
                TransactionStatus.Pending => "?? ??????",
                TransactionStatus.Completed => "????? ???",
                TransactionStatus.Failed => "??????",
                TransactionStatus.Refunded => "?????? ???",
                TransactionStatus.Cancelled => "??? ???",
                _ => status.ToString()
            };
        }

        private static string GetPaymentMethodDisplay(Domain.Accounting.PaymentMethod method)
        {
            return method switch
            {
                Domain.Accounting.PaymentMethod.Online => "??????",
                Domain.Accounting.PaymentMethod.Wallet => "??? ???",
                Domain.Accounting.PaymentMethod.Cash => "????",
                Domain.Accounting.PaymentMethod.Transfer => "?????? ?????",
                _ => method.ToString()
            };
        }

        private static string GetCommentStatusDisplay(CommentStatus status)
        {
            return status switch
            {
                CommentStatus.Pending => "?? ?????? ?????",
                CommentStatus.Approved => "????? ???",
                CommentStatus.Rejected => "?? ???",
                _ => status.ToString()
            };
        }

        private static string GetSessionStatusDisplay(SessionStatus status)
        {
            return status switch
            {
                SessionStatus.Scheduled => "????????? ???",
                SessionStatus.InProgress => "?? ??? ???????",
                SessionStatus.Completed => "????? ???",
                SessionStatus.Cancelled => "??? ???",
                _ => status.ToString()
            };
        }

        private static string GetAttendanceStatusDisplay(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "????",
                AttendanceStatus.Absent => "????",
                AttendanceStatus.Late => "?????",
                _ => status.ToString()
            };
        }


        private static string GetEnrollmentStatusDisplay(EnrollmentStatus status)
        {
            return status switch
            {
                EnrollmentStatus.Active => "????",
                EnrollmentStatus.Completed => "????? ???",
                EnrollmentStatus.Cancelled => "??? ???",
                EnrollmentStatus.Suspended => "????? ???",
                _ => status.ToString()
            };
        }

        private static string GetInstallmentStatusDisplay(InstallmentStatus status)
        {
            return status switch
            {
                InstallmentStatus.Unpaid => "?????? ????",
                InstallmentStatus.Partial => "?????? ????",
                InstallmentStatus.Paid => "?????? ???",
                InstallmentStatus.Overdue => "????",
                _ => status.ToString()
            };
        }

        private static string GetEnrollmentPaymentMethodDisplay(Domain.Payments.EnrollmentPaymentMethod method)
        {
            return method switch
            {
                Domain.Payments.EnrollmentPaymentMethod.Online => "??????",
                Domain.Payments.EnrollmentPaymentMethod.Cash => "????",
                Domain.Payments.EnrollmentPaymentMethod.Transfer => "?????? ?????",
                Domain.Payments.EnrollmentPaymentMethod.Cheque => "??",
                _ => method.ToString()
            };
        }

        // ?????? ???? ???? ????? ?????? ?? ???? CourseType
        private static string? GetClassLocationForCourseType(CourseType courseType, string location)
        {
            return courseType switch
            {
                CourseType.InPerson => location,
                CourseType.Hybrid => location,
                CourseType.Online => null,
                _ => null
            };
        }

        private static string? GetLiveUrlForCourseType(CourseType courseType, string? exclusiveLiveLink)
        {
            return courseType switch
            {
                CourseType.Online => exclusiveLiveLink,
                CourseType.Hybrid => exclusiveLiveLink,
                CourseType.InPerson => null,
                _ => null
            };
        }

    }
}
