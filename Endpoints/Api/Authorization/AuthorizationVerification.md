# Authorization Policy Migration Verification

## Checklist: Ensure No Controllers Still Use Direct Roles

### ✅ Step 1: Search for Remaining Role Usage

Run these commands to find any remaining direct role usage:

```bash
# Search for [Authorize(Roles =
grep -r "\[Authorize(Roles" --include="*.cs" ./Controllers/
grep -r "\[Authorize(Roles" --include="*.cs" ./Areas/

# Search for Role. constants in controllers
grep -r "Role\." --include="*.cs" ./Controllers/
grep -r "Role\." --include="*.cs" ./Areas/
```

### ✅ Step 2: Verify Policy Registration

Ensure `builder.Services.AddAuthorization(options => options.AddPardisPolicies());` is in Program.cs

### ✅ Step 3: Build and Test

```bash
dotnet build
dotnet test
```

## Expected Access Control Matrix

| Controller/Action                   | Policy                            | Required Roles                                   | Test Endpoint                                   |
| ----------------------------------- | --------------------------------- | ------------------------------------------------ | ----------------------------------------------- |
| **AdminSystemController**           | `AdminSystem.Access`              | ITManager, Admin, Manager                        | `GET /api/admin/system/settings`                |
| **AdminCoursesController**          | `AdminCourses.Access`             | Instructor, EducationExpert, CourseSupport       | `GET /api/admin/courses/my-courses`             |
| **UserController**                  | `UserManagement.Access`           | Admin, Manager                                   | `GET /api/users`                                |
| **UserController.UpdateRoles**      | `UserManagement.UpdateRoles`      | Admin only                                       | `PUT /api/users/{id}/roles`                     |
| **StudentsController**              | `StudentManagement.Access`        | Admin, Manager                                   | `GET /api/admin/students`                       |
| **ReportsController**               | `Reports.Access`                  | Admin, Manager, GeneralManager, FinancialManager | `GET /api/admin/reports`                        |
| **PaymentManagementController**     | `PaymentManagement.Access`        | Admin only                                       | `GET /api/admin/payments`                       |
| **DashboardController**             | `Dashboard.Access`                | Admin, Manager                                   | `GET /api/dashboard`                            |
| **CommentsController (Admin)**      | `CommentManagement.Access`        | Admin, Manager, Instructor                       | `GET /api/admin/comments`                       |
| **CategoryController**              | `CategoryManagement.Access`       | Admin, Manager                                   | `POST /api/categories`                          |
| **CategoryController.Delete**       | `CategoryManagement.Delete`       | Admin only                                       | `DELETE /api/categories/{id}`                   |
| **CourseController.Create**         | `CourseManagement.Create`         | Admin, Manager                                   | `POST /api/admin/courses`                       |
| **CourseController.Update**         | `CourseManagement.Update`         | Admin, Manager, Instructor                       | `PUT /api/admin/courses/{id}`                   |
| **CourseController.ViewFinancials** | `CourseManagement.ViewFinancials` | Admin, Manager                                   | `GET /api/admin/courses/{id}/financial-summary` |
| **ShoppingController**              | `Shopping.Access`                 | Authenticated users                              | `GET /api/me/cart`                              |
| **PaymentAttemptsController**       | `PaymentAttempts.Access`          | Authenticated users                              | `GET /api/me/payment-attempts`                  |
| **PaymentsController**              | `Payments.Access`                 | Authenticated users                              | `GET /api/payments`                             |
| **PaymentsController.AdminActions** | `Payments.AdminActions`           | Admin only                                       | `GET /api/payments/admin/manual`                |

## Integration Test Examples

### Test 1: Admin System Access

```csharp
[Test]
public async Task AdminSystemController_RequiresCorrectPolicy()
{
    // Arrange: User with ITManager role
    var token = GenerateJwtToken("testuser", new[] { "ITManager" });

    // Act
    var response = await _client.GetAsync("/api/admin/system/settings",
        headers: new { Authorization = $"Bearer {token}" });

    // Assert
    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
}

[Test]
public async Task AdminSystemController_DeniesUnauthorizedRoles()
{
    // Arrange: User with Student role
    var token = GenerateJwtToken("testuser", new[] { "Student" });

    // Act
    var response = await _client.GetAsync("/api/admin/system/settings",
        headers: new { Authorization = $"Bearer {token}" });

    // Assert
    Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
}
```

### Test 2: User Management Role Separation

```csharp
[Test]
public async Task UserController_UpdateRoles_RequiresAdminOnly()
{
    // Arrange: User with Manager role (should be denied)
    var token = GenerateJwtToken("manager", new[] { "Manager" });

    // Act
    var response = await _client.PutAsync("/api/users/123/roles",
        content: JsonContent.Create(new { roles = new[] { "Student" } }),
        headers: new { Authorization = $"Bearer {token}" });

    // Assert
    Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
}
```

### Test 3: Shopping Access for Authenticated Users

```csharp
[Test]
public async Task ShoppingController_AllowsAnyAuthenticatedUser()
{
    // Arrange: User with Student role
    var token = GenerateJwtToken("student", new[] { "Student" });

    // Act
    var response = await _client.GetAsync("/api/me/cart",
        headers: new { Authorization = $"Bearer {token}" });

    // Assert
    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
}
```

## Manual Testing Checklist

### ✅ Test with Different User Roles

1. **Admin User**: Should access all endpoints
2. **Manager User**: Should access management endpoints but not admin-only actions
3. **Instructor User**: Should access course-related endpoints
4. **Student User**: Should only access user-facing endpoints
5. **Unauthenticated**: Should be denied access to protected endpoints

### ✅ Test Specific Scenarios

- [ ] Admin can update user roles
- [ ] Manager cannot update user roles
- [ ] Instructor can view course students
- [ ] Student cannot access admin endpoints
- [ ] Unauthenticated user can view public comments
- [ ] All authenticated users can access shopping cart

### ✅ Verify Error Responses

- [ ] 401 Unauthorized for missing/invalid tokens
- [ ] 403 Forbidden for insufficient permissions
- [ ] Proper error messages in response body

## Rollback Plan

If issues are found:

1. **Immediate Rollback**: Revert Program.cs changes and controller imports
2. **Partial Rollback**: Keep policies but revert specific controllers
3. **Fix Forward**: Update policy definitions in AuthorizationExtensions.cs

## Success Criteria

- ✅ No compilation errors
- ✅ All existing functionality works
- ✅ No direct role strings in controllers
- ✅ Proper 401/403 responses
- ✅ All tests pass
- ✅ Security audit shows no regressions
