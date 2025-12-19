using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Users;
using Api.Controllers;

namespace Api.Controllers;

/// <summary>
/// کنترلر مدیریت ثبت‌نام‌ها و اقساط
/// </summary>
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : BaseController
{
    public EnrollmentsController(ILogger<EnrollmentsController> logger) : base(logger)
    {
    }

    /// <summary>
    /// دریافت اقساط یک ثبت‌نام
    /// </summary>
    [HttpGet("{enrollmentId}/installments")]
    public async Task<IActionResult> GetEnrollmentInstallments(Guid enrollmentId)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            // TODO: بررسی دسترسی کاربر به این ثبت‌نام
            
            // TODO: پیاده‌سازی Query برای دریافت اقساط
            var mockData = new
            {
                enrollment = new
                {
                    id = enrollmentId,
                    courseId = Guid.NewGuid(),
                    studentId = userId,
                    totalAmount = 5000000,
                    paidAmount = 2000000,
                    paymentStatus = "Partial",
                    enrollmentStatus = "Active",
                    enrollmentDate = DateTime.UtcNow.AddDays(-30),
                    isInstallmentAllowed = true,
                    installmentCount = 4
                },
                installments = new[]
                {
                    new
                    {
                        id = Guid.NewGuid(),
                        installmentNumber = 1,
                        amount = 1250000,
                        paidAmount = 1250000,
                        dueDate = DateTime.UtcNow.AddDays(-15),
                        status = "Paid"
                    },
                    new
                    {
                        id = Guid.NewGuid(),
                        installmentNumber = 2,
                        amount = 1250000,
                        paidAmount = 750000,
                        dueDate = DateTime.UtcNow.AddDays(15),
                        status = "Partial"
                    },
                    new
                    {
                        id = Guid.NewGuid(),
                        installmentNumber = 3,
                        amount = 1250000,
                        paidAmount = 0,
                        dueDate = DateTime.UtcNow.AddDays(45),
                        status = "Pending"
                    },
                    new
                    {
                        id = Guid.NewGuid(),
                        installmentNumber = 4,
                        amount = 1250000,
                        paidAmount = 0,
                        dueDate = DateTime.UtcNow.AddDays(75),
                        status = "Pending"
                    }
                }
            };
            
            return SuccessResponse(mockData, "اقساط ثبت‌نام با موفقیت دریافت شدند");
        }, "خطا در دریافت اقساط ثبت‌نام");
    }
}