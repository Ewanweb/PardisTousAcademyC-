namespace Api.Authorization;

/// <summary>
/// Central definition of all authorization policies for the Pardis Academy API
/// </summary>
public static class Policies
{
    /// <summary>
    /// System administration policies - for IT managers and system administrators
    /// </summary>
    public static class AdminSystem
    {
        public const string Access = "AdminSystem.Access";
    }

    /// <summary>
    /// Course administration policies - for instructors and education staff
    /// </summary>
    public static class AdminCourses
    {
        public const string Access = "AdminCourses.Access";
    }

    /// <summary>
    /// User management policies - for admin and manager roles
    /// </summary>
    public static class UserManagement
    {
        public const string Access = "UserManagement.Access";
        public const string UpdateRoles = "UserManagement.UpdateRoles";
    }

    /// <summary>
    /// System administration policies - for IT managers and system administrators
    /// </summary>
    public static class Accounting
    {
        public const string Access = "Accounting.Access";
    }

    /// <summary>
    /// Student management policies - for admin and manager roles
    /// </summary>
    public static class StudentManagement
    {
        public const string Access = "StudentManagement.Access";
    }

    /// <summary>
    /// Reports and analytics policies - for management and financial staff
    /// </summary>
    public static class Reports
    {
        public const string Access = "Reports.Access";
    }

    /// <summary>
    /// Payment management policies - for admin only
    /// </summary>
    public static class PaymentManagement
    {
        public const string Access = "PaymentManagement.Access";
        public const string AdminActions = "PaymentManagement.AdminActions";
    }

    /// <summary>
    /// Dashboard access policies - for admin and manager roles
    /// </summary>
    public static class Dashboard
    {
        public const string Access = "Dashboard.Access";
    }

    /// <summary>
    /// Comment management policies - for admin, manager, and instructor roles
    /// </summary>
    public static class CommentManagement
    {
        public const string Access = "CommentManagement.Access";
    }

    /// <summary>
    /// Category management policies - for admin and manager roles
    /// </summary>
    public static class CategoryManagement
    {
        public const string Access = "CategoryManagement.Access";
        public const string Delete = "CategoryManagement.Delete";
    }

    /// <summary>
    /// Course management policies - mixed permissions per action
    /// </summary>
    public static class CourseManagement
    {
        public const string Create = "CourseManagement.Create";
        public const string Update = "CourseManagement.Update";
        public const string Delete = "CourseManagement.Delete";
        public const string ViewStudents = "CourseManagement.ViewStudents";
        public const string ViewFinancials = "CourseManagement.ViewFinancials";
    }

    /// <summary>
    /// Shopping and checkout policies - for authenticated users
    /// </summary>
    public static class Shopping
    {
        public const string Access = "Shopping.Access";
    }

    /// <summary>
    /// Payment attempts policies - for authenticated users
    /// </summary>
    public static class PaymentAttempts
    {
        public const string Access = "PaymentAttempts.Access";
    }

    /// <summary>
    /// General payments policies - for authenticated users with some admin actions
    /// </summary>
    public static class Payments
    {
        public const string Access = "Payments.Access";
        public const string AdminActions = "Payments.AdminActions";
    }

    /// <summary>
    /// Comments policies - for authenticated users
    /// </summary>
    public static class Comments
    {
        public const string Access = "Comments.Access";
    }

    /// <summary>
    /// Enrollments policies - for authenticated users
    /// </summary>
    public static class Enrollments
    {
        public const string Access = "Enrollments.Access";
    }

    /// <summary>
    /// Course schedule policies - for authenticated users
    /// </summary>
    public static class CourseSchedule
    {
        public const string Access = "CourseSchedule.Access";
    }

    /// <summary>
    /// System logs policies - for authenticated users
    /// </summary>
    public static class Logs
    {
        public const string Access = "Logs.Access";
    }

    /// <summary>
    /// Hero slides management policies - for authenticated users
    /// </summary>
    public static class HeroSlides
    {
        public const string Access = "HeroSlides.Access";
    }

    /// <summary>
    /// Success stories management policies - for authenticated users
    /// </summary>
    public static class SuccessStories
    {
        public const string Access = "SuccessStories.Access";
    }

    /// <summary>
    /// General courses policies - for authenticated users
    /// </summary>
    public static class Courses
    {
        public const string Access = "Courses.Access";
    }

    /// <summary>
    /// Authentication policies - for authenticated users
    /// </summary>
    public static class Auth
    {
        public const string Access = "Auth.Access";
    }
}