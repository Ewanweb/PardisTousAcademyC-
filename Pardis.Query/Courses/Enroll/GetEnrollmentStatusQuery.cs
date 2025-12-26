using MediatR;

namespace Pardis.Query.Courses.Enroll
{
    /// <summary>
    /// Query برای بررسی وضعیت ثبت‌نام کاربر در یک دوره
    /// </summary>
    public class GetEnrollmentStatusQuery : IRequest<EnrollmentStatusResult>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// شناسه دوره
        /// </summary>
        public required Guid CourseId { get; set; }
    }

    /// <summary>
    /// نتیجه بررسی وضعیت ثبت‌نام
    /// </summary>
    public class EnrollmentStatusResult
    {
        /// <summary>
        /// آیا کاربر در دوره ثبت‌نام کرده است؟
        /// </summary>
        public bool IsEnrolled { get; set; }

        /// <summary>
        /// تاریخ ثبت‌نام
        /// </summary>
        public DateTime? EnrollmentDate { get; set; }

        /// <summary>
        /// وضعیت پرداخت
        /// </summary>
        public string PaymentStatus { get; set; } = "NotPaid";

        /// <summary>
        /// مبلغ پرداخت شده
        /// </summary>
        public long PaidAmount { get; set; }

        /// <summary>
        /// مبلغ کل دوره
        /// </summary>
        public long TotalAmount { get; set; }

        /// <summary>
        /// آیا امکان پرداخت قسطی وجود دارد؟
        /// </summary>
        public bool CanPayInstallment { get; set; }
    }
}