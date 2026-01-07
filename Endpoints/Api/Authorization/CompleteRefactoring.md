# Complete Authorization Refactoring Guide

## Remaining Controllers to Refactor

Based on the scan, here are all controllers that still need policy-based authorization:

### 1. PaymentsController - Admin Actions

```csharp
// BEFORE:
[Authorize(Roles = "Admin")]

// AFTER:
[Authorize(Policy = Policies.Payments.AdminActions)]
```

### 2. AdminCoursesController - Already partially done, needs import fix

```csharp
// Add import:
using Api.Authorization;

// BEFORE:
[Authorize(Roles = $"{Role.Instructor},{Role.EducationExpert},{Role.CourseSupport}")]

// AFTER:
[Authorize(Policy = Policies.AdminCourses.Access)]
```

### 3. AccountingController

```csharp
// Add import:
using Api.Authorization;

// BEFORE:
[Authorize(Roles = Role.AccountingRoles)]

// AFTER:
[Authorize(Policy = Policies.Reports.Access)] // Or create new Accounting policy
```

### 4. AttendanceController

```csharp
// Add import:
using Api.Authorization;

// BEFORE:
[Authorize(Roles = Role.Admin + "," + Role.Instructor)]

// AFTER:
[Authorize(Policy = Policies.CommentManagement.Access)] // Or create new Attendance policy
```

### 5. ReportsController

```csharp
// Add import:
using Api.Authorization;

// BEFORE:
[Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.GeneralManager + "," + Role.FinancialManager)]

// AFTER:
[Authorize(Policy = Policies.Reports.Access)]
```

### 6. StudentsController

```csharp
// Add import:
using Api.Authorization;

// BEFORE:
[Authorize(Roles = Role.Admin + "," + Role.Manager)]

// AFTER:
[Authorize(Policy = Policies.StudentManagement.Access)]
```

### 7. PaymentManagementController

```csharp
// Add import:
using Api.Authorization;

// BEFORE:
[Authorize(Roles = "Admin")]

// AFTER:
[Authorize(Policy = Policies.PaymentManagement.Access)]
```

### 8. DashboardController

```csharp
// Add import:
using Api.Authorization;

// BEFORE:
[Authorize(Roles = "Admin,Manager")]

// AFTER:
[Authorize(Policy = Policies.Dashboard.Access)]
```

### 9. CommentsController (Admin)

```csharp
// Add import:
using Api.Authorization;

// BEFORE:
[Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]

// AFTER:
[Authorize(Policy = Policies.CommentManagement.Access)]
```

### 10. CategoryController - Mixed Permissions

```csharp
// Add import:
using Api.Authorization;

// BEFORE (Controller level):
[Authorize]
[Authorize(Roles = "Admin,Manager")]

// AFTER (Controller level):
[Authorize(Policy = Policies.CategoryManagement.Access)]

// BEFORE (Action level):
[Authorize(Roles = Role.Admin)]

// AFTER (Action level):
[Authorize(Policy = Policies.CategoryManagement.Delete)]
```

### 11. CourseController (Admin) - Multiple Action Policies

```csharp
// Add import:
using Api.Authorization;

// Actions to refactor:
[HttpPost()]
[Authorize(Policy = Policies.CourseManagement.Create)]

[HttpPut("{id}")]
[Authorize(Policy = Policies.CourseManagement.Update)]

[HttpDelete("{id}")]
[Authorize(Policy = Policies.CourseManagement.Delete)]

[HttpPost("{id}/restore")]
[Authorize(Policy = Policies.CourseManagement.Delete)]

[HttpDelete("{id}/force")]
[Authorize(Policy = Policies.CourseManagement.Delete)]

[HttpGet("{courseId}/students")]
[Authorize(Policy = Policies.CourseManagement.ViewStudents)]

[HttpGet("{courseId}/financial-summary")]
[Authorize(Policy = Policies.CourseManagement.ViewFinancials)]
```

## Additional Policies Needed

Add these to AuthorizationExtensions.cs:

```csharp
// Accounting policies
options.AddPolicy(Policies.Accounting.Access, policy =>
    policy.RequireRole(Roles.Admin, Roles.Manager, Roles.GeneralManager, Roles.FinancialManager, Roles.Accountant));

// Attendance policies
options.AddPolicy(Policies.Attendance.Access, policy =>
    policy.RequireRole(Roles.Admin, Roles.Instructor));
```

And add to Policies.cs:

```csharp
/// <summary>
/// Accounting policies - for financial staff
/// </summary>
public static class Accounting
{
    public const string Access = "Accounting.Access";
}

/// <summary>
/// Attendance policies - for admin and instructors
/// </summary>
public static class Attendance
{
    public const string Access = "Attendance.Access";
}
```

## Automated Refactoring Commands

Run these PowerShell commands to complete the refactoring:

```powershell
# 1. Find all remaining role usage
Get-ChildItem -Path "Controllers","Areas" -Recurse -Filter "*.cs" | Select-String -Pattern "\[Authorize\(Roles"

# 2. Find Role. constants usage
Get-ChildItem -Path "Controllers","Areas" -Recurse -Filter "*.cs" | Select-String -Pattern "Role\."

# 3. Verify no hardcoded role strings
Get-ChildItem -Path "Controllers","Areas" -Recurse -Filter "*.cs" | Select-String -Pattern '"Admin"'
```

## Final Verification Steps

1. **Build Check**: `dotnet build` should succeed
2. **Role Usage Check**: No `[Authorize(Roles = ` should remain
3. **Import Check**: All controllers should have `using Api.Authorization;`
4. **Policy Coverage**: All endpoints should have appropriate policies
5. **Test Access**: Verify role-based access still works correctly

## Testing Matrix

| User Role  | Should Access            | Should NOT Access          |
| ---------- | ------------------------ | -------------------------- |
| Admin      | All endpoints            | None                       |
| Manager    | Management endpoints     | Admin-only actions         |
| Instructor | Course-related endpoints | Financial/User management  |
| Student    | User-facing endpoints    | Admin/Management endpoints |

## Success Criteria

- ✅ Zero `[Authorize(Roles = ` in controllers
- ✅ Zero `Role.` constants in controllers
- ✅ All policies registered in Program.cs
- ✅ Build succeeds without errors
- ✅ All existing functionality preserved
- ✅ Proper 401/403 responses for unauthorized access
