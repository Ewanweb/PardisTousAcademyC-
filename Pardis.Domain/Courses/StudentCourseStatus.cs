namespace Pardis.Domain.Courses;

public enum StudentCourseStatus
{
    Active = 1,       // در حال تحصیل
    Completed = 2,    // فارغ‌التحصیل
    Dropped = 3,      // انصراف داده
    Suspended = 4,    // تعلیق شده (مثلاً غیبت زیاد)
    Failed = 5        // مردود
}