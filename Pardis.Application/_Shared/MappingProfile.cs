using AutoMapper;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Seo;
using Pardis.Domain.Users;
using Pardis.Domain.Accounting;
using Pardis.Domain.Comments;
using Pardis.Domain.Attendance;
using Pardis.Domain.Payments;
using Pardis.Domain.Sliders;
using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Dto.Accounting;
using Pardis.Domain.Dto.Comments;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Dto.Sliders;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application._Shared
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // تبدیل SeoMetadata به SeoDto
            CreateMap<SeoMetadata, SeoDto>().ReverseMap();

            // Slider Mappings
            CreateMap<HeroSlide, HeroSlideResource>()
                .ForMember(dest => dest.TimeRemaining, opt => opt.MapFrom(src => src.GetTimeRemaining()))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()));
                
            CreateMap<HeroSlide, HeroSlideListResource>()
                .ForMember(dest => dest.TimeRemaining, opt => opt.MapFrom(src => src.GetTimeRemaining()))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()));
                
            CreateMap<SuccessStory, SuccessStoryResource>()
                .ForMember(dest => dest.TimeRemaining, opt => opt.MapFrom(src => src.GetTimeRemaining()))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()));
                
            CreateMap<SuccessStory, SuccessStoryListResource>()
                .ForMember(dest => dest.TimeRemaining, opt => opt.MapFrom(src => src.GetTimeRemaining()))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()));

            // تبدیل User به UserResource (بدون Courses برای جلوگیری از circular reference)
            CreateMap<User, UserResource>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Roles رو جداگانه handle کن
                .ReverseMap();

            // تبدیل Category به CategoryResource
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
            // Ignore کردن Children چون در این DTO لود نشده‌اند

            // تبدیل Course به CourseResource
            CreateMap<CourseSection, CourseSectionDto>().ReverseMap();

            // تبدیل CourseSchedule به CourseScheduleDto
            CreateMap<CourseSchedule, CourseScheduleDto>()
                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => src.GetDayName()))
                .ForMember(dest => dest.TimeRange, opt => opt.MapFrom(src => $"{src.StartTime:HH:mm}-{src.EndTime:HH:mm}"))
                .ForMember(dest => dest.FullScheduleText, opt => opt.MapFrom(src => src.GetFullScheduleText()))
                .ForMember(dest => dest.RemainingCapacity, opt => opt.MapFrom(src => src.RemainingCapacity))
                .ForMember(dest => dest.HasCapacity, opt => opt.MapFrom(src => src.HasCapacity));

            // تبدیل User به InstructorBasicDto (بدون Courses)
            CreateMap<User, InstructorBasicDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile));

            CreateMap<UserCourse, CourseResource>()
                // الف) ابتدا تمام اطلاعات را از "Course" بردار (Title, Price, Image, ...)
                // این کار باعث می‌شود از کانفیگ "Course -> CourseResource" استفاده کند
                .IncludeMembers(s => s.Course)

                // ب) سپس فیلدهای اختصاصی را از "UserCourse" بازنویسی کن
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CourseId)) // آی‌دی دوره را بگذار نه آی‌دی جدول واسط
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted)) // وضعیت تکمیل دانشجو
                .ForMember(dest => dest.IsStarted, opt => opt.MapFrom(src => true)) // چون ثبت‌نام کرده، یعنی شروع کرده
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Course.CreatedAt)); // تاریخ ایجاد دوره (نه تاریخ ثبت‌نام)
            // اگر خواستید تاریخ ثبت‌نام را برگردانید، باید یک فیلد EnrolledAt به CourseResource اضافه کنید

            // تبدیل Course به CourseResource
            CreateMap<Course, CourseResource>()
                .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections))
                .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Instructor))
                .ReverseMap();

            // تبدیل Transaction به TransactionDto
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.FullName ?? src.User.UserName ?? "نامشخص"))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.User.Email ?? ""))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetTransactionStatusDisplay(src.Status)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
                .ForMember(dest => dest.MethodDisplay, opt => opt.MapFrom(src => GetPaymentMethodDisplay(src.Method)));

            // ✅ Comments Mappings
            CreateMap<CourseComment, CourseCommentDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.FullName ?? src.User.UserName ?? "نامشخص"))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetCommentStatusDisplay(src.Status)))
                .ForMember(dest => dest.ReviewedByUserName, opt => opt.MapFrom(src => src.ReviewedByUser != null ? src.ReviewedByUser.FullName ?? src.ReviewedByUser.UserName : null));

            // ✅ Attendance Mappings
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
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName ?? src.Student.UserName ?? "نامشخص"))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetAttendanceStatusDisplay(src.Status)))
                .ForMember(dest => dest.AttendanceDuration, opt => opt.MapFrom(src => src.GetAttendanceDuration()))
                .ForMember(dest => dest.RecordedByUserName, opt => opt.MapFrom(src => src.RecordedByUser != null ? src.RecordedByUser.FullName ?? src.RecordedByUser.UserName : null));

            // ✅ Payments Mappings
            CreateMap<CourseEnrollment, CourseEnrollmentDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName ?? src.Student.UserName ?? "نامشخص"))
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => src.GetRemainingAmount()))
                .ForMember(dest => dest.PaymentStatusDisplay, opt => opt.MapFrom(src => GetPaymentStatusDisplay(src.PaymentStatus)))
                .ForMember(dest => dest.EnrollmentStatusDisplay, opt => opt.MapFrom(src => GetEnrollmentStatusDisplay(src.EnrollmentStatus)))
                .ForMember(dest => dest.PaymentPercentage, opt => opt.MapFrom(src => src.GetPaymentPercentage()))
                .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.InstallmentPayments));

            CreateMap<InstallmentPayment, InstallmentPaymentDto>()
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => src.GetRemainingAmount()))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetInstallmentStatusDisplay(src.Status)))
                .ForMember(dest => dest.PaymentMethodDisplay, opt => opt.MapFrom(src => src.PaymentMethod.HasValue ? GetEnrollmentPaymentMethodDisplay(src.PaymentMethod.Value) : null))
                .ForMember(dest => dest.DaysUntilDue, opt => opt.MapFrom(src => src.GetDaysUntilDue()))
                .ForMember(dest => dest.DaysOverdue, opt => opt.MapFrom(src => src.GetDaysOverdue()));
        }

        private static string GetTransactionStatusDisplay(TransactionStatus status)
        {
            return status switch
            {
                TransactionStatus.Pending => "در انتظار",
                TransactionStatus.Completed => "تکمیل شده",
                TransactionStatus.Failed => "ناموفق",
                TransactionStatus.Refunded => "بازگشت وجه",
                TransactionStatus.Cancelled => "لغو شده",
                _ => status.ToString()
            };
        }

        private static string GetPaymentMethodDisplay(Domain.Accounting.PaymentMethod method)
        {
            return method switch
            {
                Domain.Accounting.PaymentMethod.Online => "آنلاین",
                Domain.Accounting.PaymentMethod.Wallet => "کیف پول",
                Domain.Accounting.PaymentMethod.Cash => "نقدی",
                Domain.Accounting.PaymentMethod.Transfer => "انتقال بانکی",
                _ => method.ToString()
            };
        }

        private static string GetCommentStatusDisplay(CommentStatus status)
        {
            return status switch
            {
                CommentStatus.Pending => "در انتظار تأیید",
                CommentStatus.Approved => "تأیید شده",
                CommentStatus.Rejected => "رد شده",
                _ => status.ToString()
            };
        }

        private static string GetSessionStatusDisplay(SessionStatus status)
        {
            return status switch
            {
                SessionStatus.Scheduled => "زمان‌بندی شده",
                SessionStatus.InProgress => "در حال برگزاری",
                SessionStatus.Completed => "تکمیل شده",
                SessionStatus.Cancelled => "لغو شده",
                _ => status.ToString()
            };
        }

        private static string GetAttendanceStatusDisplay(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "حاضر",
                AttendanceStatus.Absent => "غایب",
                AttendanceStatus.Late => "تأخیر",
                _ => status.ToString()
            };
        }

        private static string GetPaymentStatusDisplay(Domain.Payments.PaymentStatus status)
        {
            return status switch
            {
                Domain.Payments.PaymentStatus.Unpaid => "پرداخت نشده",
                Domain.Payments.PaymentStatus.Partial => "پرداخت جزئی",
                Domain.Payments.PaymentStatus.Paid => "پرداخت کامل",
                _ => status.ToString()
            };
        }

        private static string GetEnrollmentStatusDisplay(EnrollmentStatus status)
        {
            return status switch
            {
                EnrollmentStatus.Active => "فعال",
                EnrollmentStatus.Completed => "تکمیل شده",
                EnrollmentStatus.Cancelled => "لغو شده",
                EnrollmentStatus.Suspended => "تعلیق شده",
                _ => status.ToString()
            };
        }

        private static string GetInstallmentStatusDisplay(InstallmentStatus status)
        {
            return status switch
            {
                InstallmentStatus.Unpaid => "پرداخت نشده",
                InstallmentStatus.Partial => "پرداخت جزئی",
                InstallmentStatus.Paid => "پرداخت شده",
                InstallmentStatus.Overdue => "معوق",
                _ => status.ToString()
            };
        }

        private static string GetEnrollmentPaymentMethodDisplay(Domain.Payments.EnrollmentPaymentMethod method)
        {
            return method switch
            {
                Domain.Payments.EnrollmentPaymentMethod.Online => "آنلاین",
                Domain.Payments.EnrollmentPaymentMethod.Cash => "نقدی",
                Domain.Payments.EnrollmentPaymentMethod.Transfer => "انتقال بانکی",
                Domain.Payments.EnrollmentPaymentMethod.Cheque => "چک",
                _ => method.ToString()
            };
        }
    }
}
