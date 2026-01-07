using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

/// <summary>
/// Extension methods for registering authorization policies
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Registers all Pardis Academy authorization policies
    /// </summary>
    /// <param name="options">Authorization options</param>
    public static void AddPardisPolicies(this AuthorizationOptions options)
    {
        // System Administration Policies
        options.AddPolicy(Policies.AdminSystem.Access, policy =>
            policy.RequireRole(Roles.ITManager, Roles.Admin, Roles.Manager));

        // Course Administration Policies
        options.AddPolicy(Policies.AdminCourses.Access, policy =>
            policy.RequireRole(Roles.Instructor, Roles.EducationExpert, Roles.CourseSupport));

        // User Management Policies
        options.AddPolicy(Policies.UserManagement.Access, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager));

        options.AddPolicy(Policies.UserManagement.UpdateRoles, policy =>
            policy.RequireRole(Roles.Admin)); // Only Admin can update roles

        // Student Management Policies
        options.AddPolicy(Policies.StudentManagement.Access, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager));

        // Reports Policies
        options.AddPolicy(Policies.Reports.Access, policy =>
            policy.RequireRole(Roles.Manager, Roles.Admin, Roles.GeneralManager, Roles.FinancialManager));

        // Payment Management Policies
        options.AddPolicy(Policies.PaymentManagement.Access, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager));

        options.AddPolicy(Policies.PaymentManagement.AdminActions, policy =>
            policy.RequireRole(Roles.Manager));

        // Dashboard Policies
        options.AddPolicy(Policies.Dashboard.Access, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager));

        // Comment Management Policies
        options.AddPolicy(Policies.CommentManagement.Access, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager, Roles.Instructor));

        // Category Management Policies
        options.AddPolicy(Policies.CategoryManagement.Access, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager));

        options.AddPolicy(Policies.CategoryManagement.Delete, policy =>
            policy.RequireRole(Roles.Admin)); // Only Admin can delete categories

        // Course Management Policies (granular permissions)
        options.AddPolicy(Policies.CourseManagement.Create, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager));

        options.AddPolicy(Policies.CourseManagement.Update, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager, Roles.Instructor));

        options.AddPolicy(Policies.CourseManagement.Delete, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager, Roles.Instructor));

        options.AddPolicy(Policies.CourseManagement.ViewStudents, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager, Roles.Instructor));

        options.AddPolicy(Policies.CourseManagement.ViewFinancials, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager));

        // General Payments Policies
        options.AddPolicy(Policies.Payments.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.Payments.AdminActions, policy =>
            policy.RequireRole(Roles.Admin, Roles.Manager));

        // Authenticated User Policies (for controllers that just need [Authorize])
        options.AddPolicy(Policies.Shopping.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.PaymentAttempts.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.Comments.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.Enrollments.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.CourseSchedule.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.Logs.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.HeroSlides.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.SuccessStories.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.Courses.Access, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Policies.Auth.Access, policy =>
            policy.RequireAuthenticatedUser());
    }
}