namespace Api.Authorization;

/// <summary>
/// Central definition of all role constants used in authorization policies
/// This mirrors the roles defined in Pardis.Domain.Users.Role but provides a centralized reference
/// </summary>
public static class Roles
{
    // Base roles
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string User = "User";

    // Organizational roles
    public const string FinancialManager = "FinancialManager";
    public const string Instructor = "Instructor";
    public const string Student = "Student";
    public const string ITManager = "ITManager";
    public const string MarketingManager = "MarketingManager";
    public const string EducationManager = "EducationManager";
    public const string Accountant = "Accountant";
    public const string GeneralManager = "GeneralManager";
    public const string DepartmentManager = "DepartmentManager";
    public const string CourseSupport = "CourseSupport";
    public const string Marketer = "Marketer";
    public const string InternalManager = "InternalManager";
    public const string EducationExpert = "EducationExpert";

    // Role groups for common combinations
    public static class Groups
    {
        public const string SystemAdmins = Admin + "," + ITManager + "," + Manager;
        public const string UserManagers = Admin + "," + Manager;
        public const string CourseStaff = Instructor + "," + EducationExpert + "," + CourseSupport;
        public const string ReportsViewers = Admin + "," + Manager + "," + GeneralManager + "," + FinancialManager;
        public const string CommentModerators = Admin + "," + Manager + "," + Instructor;
        public const string CategoryManagers = Admin + "," + Manager;
        public const string CourseCreators = Admin + "," + Manager;
        public const string CourseEditors = Admin + "," + Manager + "," + Instructor;
        public const string FinancialViewers = Admin + "," + Manager;
    }
}