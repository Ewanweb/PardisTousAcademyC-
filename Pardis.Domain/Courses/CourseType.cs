namespace Pardis.Domain.Courses;

/// <summary>
/// نوع دوره
/// </summary>
public enum CourseType
{
    /// <summary>
    /// آنلاین - دوره به صورت مجازی برگزار می‌شود
    /// </summary>
    Online = 1,

    /// <summary>
    /// حضوری - دوره در محل فیزیکی برگزار می‌شود
    /// </summary>
    InPerson = 2,

    /// <summary>
    /// ترکیبی - دوره هم آنلاین و هم حضوری دارد
    /// </summary>
    Hybrid = 3
}