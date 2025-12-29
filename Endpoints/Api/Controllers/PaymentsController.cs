using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Controllers;
using Pardis.Domain.Dto.Payments;
using Microsoft.EntityFrameworkCore;
using Pardis.Infrastructure;
using Pardis.Domain.Payments;

namespace Api.Controllers;

/// <summary>
/// کنترلر پردازش پرداخت‌ها
/// </summary>
[Route("api/payments")]
[Authorize]
public class PaymentsController : BaseController
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public PaymentsController(ILogger<PaymentsController> logger, AppDbContext context, IWebHostEnvironment environment) 
        : base(logger)
    {
        _context = context;
        _environment = environment;
    }

    /// <summary>
    /// پردازش پرداخت قسط
    /// </summary>
    [HttpPost("installment/{installmentId}")]
    public async Task<IActionResult> ProcessInstallmentPayment(Guid installmentId, [FromBody] ProcessPaymentRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            // TODO: بررسی دسترسی کاربر به این قسط
            // TODO: پیاده‌سازی Command برای پردازش پرداخت
            
            var mockData = new
            {
                paymentUrl = "https://payment-gateway.com/pay/mock-payment-url",
                paymentId = Guid.NewGuid(),
                status = "Pending",
                amount = request.Amount,
                installmentId = installmentId,
                userId = userId
            };
            
            return SuccessResponse(mockData, "درخواست پرداخت با موفقیت ایجاد شد");
        }, "خطا در پردازش پرداخت");
    }

    /// <summary>
    /// تأیید پرداخت (Callback از درگاه)
    /// </summary>
    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: پیاده‌سازی تأیید پرداخت با درگاه
            var mockData = new
            {
                paymentId = request.PaymentId,
                status = "Success",
                transactionId = request.TransactionId,
                verifiedAt = DateTime.UtcNow
            };
            
            return SuccessResponse(mockData, "پرداخت با موفقیت تأیید شد");
        }, "خطا در تأیید پرداخت");
    }

    /// <summary>
    /// دریافت وضعیت پرداخت
    /// </summary>
    [HttpGet("{paymentId}/status")]
    public async Task<IActionResult> GetPaymentStatus(Guid paymentId)
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: پیاده‌سازی Query برای وضعیت پرداخت
            var mockData = new
            {
                id = paymentId,
                status = "Success",
                amount = 1250000,
                paymentDate = DateTime.UtcNow.AddMinutes(-5),
                transactionId = "TXN123456789"
            };
            
            return SuccessResponse(mockData, "وضعیت پرداخت با موفقیت دریافت شد");
        }, "خطا در دریافت وضعیت پرداخت");
    }

    #region Manual Payment Endpoints

    /// <summary>
    /// دریافت اطلاعات کارت مقصد برای پرداخت دستی
    /// </summary>
    [HttpGet("manual/info")]
    public async Task<IActionResult> GetManualPaymentInfo()
    {
        return await ExecuteAsync(async () =>
        {
            var paymentSettings = await _context.PaymentSettings
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync();

            if (paymentSettings == null)
            {
                // اطلاعات پیش‌فرض در صورت عدم تنظیم
                var defaultInfo = new PaymentCardInfoDto
                {
                    CardNumber = "6037-9977-****-****",
                    CardHolderName = "آکادمی پردیس توس",
                    BankName = "بانک کشاورزی",
                    Description = "لطفاً پس از واریز، رسید را در همین صفحه آپلود کنید"
                };
                return SuccessResponse(defaultInfo, "اطلاعات کارت مقصد");
            }

            var cardInfo = new PaymentCardInfoDto
            {
                CardNumber = paymentSettings.CardNumber,
                CardHolderName = paymentSettings.CardHolderName,
                BankName = paymentSettings.BankName,
                Description = paymentSettings.Description
            };

            return SuccessResponse(cardInfo, "اطلاعات کارت مقصد");
        }, "خطا در دریافت اطلاعات کارت");
    }

    /// <summary>
    /// ایجاد درخواست پرداخت دستی برای دوره
    /// </summary>
    [HttpPost("courses/{courseId}/purchase/manual")]
    public async Task<IActionResult> CreateManualPaymentRequest(Guid courseId, [FromBody] CreateManualPaymentRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            // بررسی دوره رایگان
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFoundResponse("دوره یافت نشد");

            if (course.Price == 0)
            {
                // دوره رایگان - ثبت‌نام مستقیم
                var existingEnrollment = await _context.CourseEnrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == userId);

                if (existingEnrollment != null)
                    return ErrorResponse("شما قبلاً در این دوره ثبت‌نام کرده‌اید");

                var freeEnrollment = new CourseEnrollment(courseId, userId, 0);
                _context.CourseEnrollments.Add(freeEnrollment);
                await _context.SaveChangesAsync();

                return SuccessResponse(new { enrollmentId = freeEnrollment.Id }, "ثبت‌نام رایگان با موفقیت انجام شد");
            }

            // بررسی وجود درخواست پرداخت دستی قبلی در حال انتظار
            var existingRequest = await _context.ManualPaymentRequests
                .FirstOrDefaultAsync(m => m.CourseId == courseId && 
                                         m.StudentId == userId &&
                                         (m.Status == ManualPaymentStatus.PendingReceipt || 
                                          m.Status == ManualPaymentStatus.PendingApproval));

            if (existingRequest != null)
                return ErrorResponse("درخواست پرداخت دستی قبلی برای این دوره در حال بررسی است");

            // بررسی عدم ثبت‌نام قبلی
            var existingCourseEnrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == userId);

            if (existingCourseEnrollment != null)
                return ErrorResponse("شما قبلاً در این دوره ثبت‌نام کرده‌اید");

            // ایجاد درخواست پرداخت دستی
            var manualPaymentRequest = new ManualPaymentRequest(courseId, userId, request.Amount);
            _context.ManualPaymentRequests.Add(manualPaymentRequest);
            await _context.SaveChangesAsync();

            // بازگشت اطلاعات درخواست
            var dto = new ManualPaymentRequestDto
            {
                Id = manualPaymentRequest.Id,
                CourseId = manualPaymentRequest.CourseId,
                CourseName = course.Title,
                StudentId = manualPaymentRequest.StudentId,
                Amount = manualPaymentRequest.Amount,
                Status = manualPaymentRequest.Status,
                StatusDisplay = GetStatusDisplay(manualPaymentRequest.Status),
                CreatedAt = manualPaymentRequest.CreatedAt,
                UpdatedAt = manualPaymentRequest.UpdatedAt
            };

            return SuccessResponse(dto, "درخواست پرداخت دستی با موفقیت ایجاد شد");
        }, "خطا در ایجاد درخواست پرداخت");
    }

    /// <summary>
    /// آپلود رسید پرداخت دستی
    /// </summary>
    [HttpPost("manual/{paymentId}/receipt")]
    public async Task<IActionResult> UploadReceipt(Guid paymentId, [FromForm] UploadReceiptDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            // بررسی وجود درخواست پرداخت
            var paymentRequest = await _context.ManualPaymentRequests
                .Include(m => m.Course)
                .Include(m => m.Student)
                .FirstOrDefaultAsync(m => m.Id == paymentId);

            if (paymentRequest == null)
                return NotFoundResponse("درخواست پرداخت یافت نشد");

            // بررسی دسترسی کاربر
            if (paymentRequest.StudentId != userId)
                return UnauthorizedResponse("دسترسی غیرمجاز");

            // بررسی وضعیت درخواست
            if (paymentRequest.Status != ManualPaymentStatus.PendingReceipt)
                return ErrorResponse("فقط در وضعیت انتظار رسید می‌توان رسید آپلود کرد");

            // اعتبارسنجی فایل
            var validationResult = ValidateReceiptFile(request.ReceiptFile);
            if (!validationResult.IsValid)
                return ErrorResponse(validationResult.ErrorMessage);

            // آپلود فایل
            var uploadResult = await UploadFile(request.ReceiptFile, paymentRequest.Id);
            if (!uploadResult.IsSuccess)
                return ErrorResponse(uploadResult.ErrorMessage);

            // به‌روزرسانی درخواست پرداخت
            paymentRequest.UploadReceipt(uploadResult.FileUrl, uploadResult.FileName);
            await _context.SaveChangesAsync();

            // تبدیل به DTO
            var dto = MapToDto(paymentRequest);

            return SuccessResponse(dto, "رسید با موفقیت آپلود شد");
        }, "خطا در آپلود رسید");
    }

    /// <summary>
    /// دریافت لیست درخواست‌های پرداخت دستی برای ادمین
    /// </summary>
    [HttpGet("admin/manual")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetManualPaymentRequests([FromQuery] string? status = null)
    {
        return await ExecuteAsync(async () =>
        {
            var query = _context.ManualPaymentRequests
                .Include(m => m.Course)
                .Include(m => m.Student)
                .Include(m => m.AdminReviewer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ManualPaymentStatus>(status, true, out var statusEnum))
            {
                query = query.Where(m => m.Status == statusEnum);
            }

            var requests = await query
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new ManualPaymentRequestDto
                {
                    Id = m.Id,
                    CourseId = m.CourseId,
                    CourseName = m.Course.Title,
                    StudentId = m.StudentId,
                    StudentName = m.Student.UserName ?? m.Student.Email ?? "نامشخص",
                    Amount = m.Amount,
                    Status = m.Status,
                    StatusDisplay = GetStatusDisplay(m.Status),
                    ReceiptFileUrl = m.ReceiptFileUrl,
                    ReceiptFileName = m.ReceiptFileName,
                    ReceiptUploadedAt = m.ReceiptUploadedAt,
                    AdminReviewedBy = m.AdminReviewedBy,
                    AdminReviewerName = m.AdminReviewer != null ? (m.AdminReviewer.UserName ?? m.AdminReviewer.Email) : null,
                    AdminReviewedAt = m.AdminReviewedAt,
                    RejectReason = m.RejectReason,
                    Notes = m.Notes,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToListAsync();

            return SuccessResponse(requests, "لیست درخواست‌های پرداخت دستی");
        }, "خطا در دریافت درخواست‌های پرداخت");
    }

    /// <summary>
    /// تایید یا رد درخواست پرداخت دستی توسط ادمین
    /// </summary>
    [HttpPost("admin/manual/{paymentId}/review")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReviewManualPayment(Guid paymentId, [FromBody] ReviewManualPaymentDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // بررسی وجود درخواست پرداخت
                var paymentRequest = await _context.ManualPaymentRequests
                    .Include(m => m.Course)
                    .Include(m => m.Student)
                    .FirstOrDefaultAsync(m => m.Id == paymentId);

                if (paymentRequest == null)
                    return NotFoundResponse("درخواست پرداخت یافت نشد");

                // بررسی وضعیت درخواست
                if (paymentRequest.Status != ManualPaymentStatus.PendingApproval)
                    return ErrorResponse("فقط درخواست‌های در انتظار تایید قابل بررسی هستند");

                if (request.IsApproved)
                {
                    // تایید پرداخت
                    paymentRequest.Approve(userId);

                    // ایجاد ثبت‌نام در دوره
                    var existingEnrollment = await _context.CourseEnrollments
                        .FirstOrDefaultAsync(e => e.CourseId == paymentRequest.CourseId && 
                                                 e.StudentId == paymentRequest.StudentId);

                    if (existingEnrollment == null)
                    {
                        var enrollment = new CourseEnrollment(
                            paymentRequest.CourseId,
                            paymentRequest.StudentId,
                            paymentRequest.Amount);

                        // ثبت پرداخت در ثبت‌نام
                        enrollment.AddPayment(paymentRequest.Amount, $"Manual-{paymentRequest.Id}", EnrollmentPaymentMethod.Transfer);

                        _context.CourseEnrollments.Add(enrollment);
                    }
                    else
                    {
                        // اگر ثبت‌نام وجود دارد، فقط پرداخت را اضافه کن
                        existingEnrollment.AddPayment(paymentRequest.Amount, $"Manual-{paymentRequest.Id}", EnrollmentPaymentMethod.Transfer);
                    }
                }
                else
                {
                    // رد پرداخت
                    if (string.IsNullOrWhiteSpace(request.RejectReason))
                        return ErrorResponse("دلیل رد الزامی است");

                    paymentRequest.Reject(userId, request.RejectReason);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // تبدیل به DTO
                var dto = MapToDto(paymentRequest);

                var message = request.IsApproved ? "پرداخت با موفقیت تایید شد" : "پرداخت رد شد";
                return SuccessResponse(dto, message);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }, "خطا در بررسی درخواست پرداخت");
    }

    #endregion

    #region Helper Methods

    private static FileValidationResult ValidateReceiptFile(IFormFile file)
    {
        // بررسی وجود فایل
        if (file == null || file.Length == 0)
            return new FileValidationResult { IsValid = false, ErrorMessage = "فایل رسید الزامی است" };

        // بررسی سایز فایل (حداکثر 5 مگابایت)
        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
            return new FileValidationResult { IsValid = false, ErrorMessage = "حداکثر سایز فایل 5 مگابایت است" };

        // بررسی نوع فایل
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            return new FileValidationResult { IsValid = false, ErrorMessage = "فقط فایل‌های JPG، PNG و PDF مجاز هستند" };

        // بررسی Content-Type
        var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "application/pdf" };
        if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            return new FileValidationResult { IsValid = false, ErrorMessage = "نوع فایل نامعتبر است" };

        return new FileValidationResult { IsValid = true };
    }

    private async Task<FileUploadResult> UploadFile(IFormFile file, Guid paymentRequestId)
    {
        try
        {
            // ایجاد مسیر آپلود
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "receipts");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // ایجاد نام فایل امن
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{paymentRequestId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // ذخیره فایل
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // URL فایل برای دسترسی
            var fileUrl = $"/uploads/receipts/{fileName}";

            return new FileUploadResult
            {
                IsSuccess = true,
                FileUrl = fileUrl,
                FileName = file.FileName
            };
        }
        catch (Exception ex)
        {
            return new FileUploadResult
            {
                IsSuccess = false,
                ErrorMessage = $"خطا در ذخیره فایل: {ex.Message}"
            };
        }
    }

    private static ManualPaymentRequestDto MapToDto(ManualPaymentRequest paymentRequest)
    {
        return new ManualPaymentRequestDto
        {
            Id = paymentRequest.Id,
            CourseId = paymentRequest.CourseId,
            CourseName = paymentRequest.Course.Title,
            StudentId = paymentRequest.StudentId,
            StudentName = paymentRequest.Student.UserName ?? paymentRequest.Student.Email ?? "نامشخص",
            Amount = paymentRequest.Amount,
            Status = paymentRequest.Status,
            StatusDisplay = GetStatusDisplay(paymentRequest.Status),
            ReceiptFileUrl = paymentRequest.ReceiptFileUrl,
            ReceiptFileName = paymentRequest.ReceiptFileName,
            ReceiptUploadedAt = paymentRequest.ReceiptUploadedAt,
            AdminReviewedBy = paymentRequest.AdminReviewedBy,
            AdminReviewerName = paymentRequest.AdminReviewer?.UserName ?? paymentRequest.AdminReviewer?.Email,
            AdminReviewedAt = paymentRequest.AdminReviewedAt,
            RejectReason = paymentRequest.RejectReason,
            Notes = paymentRequest.Notes,
            CreatedAt = paymentRequest.CreatedAt,
            UpdatedAt = paymentRequest.UpdatedAt
        };
    }

    private static string GetStatusDisplay(ManualPaymentStatus status)
    {
        return status switch
        {
            ManualPaymentStatus.PendingReceipt => "در انتظار آپلود رسید",
            ManualPaymentStatus.PendingApproval => "در انتظار تایید ادمین",
            ManualPaymentStatus.Approved => "تایید شده",
            ManualPaymentStatus.Rejected => "رد شده",
            _ => "نامشخص"
        };
    }

    #endregion
}

// Helper classes
public class FileValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class FileUploadResult
{
    public bool IsSuccess { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

// Request DTOs
public class ProcessPaymentRequest
{
    public long Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Online, Cash, Card
    public string? Description { get; set; }
}

public class VerifyPaymentRequest
{
    public Guid PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO آپلود رسید
/// </summary>
public class UploadReceiptDto
{
    public IFormFile ReceiptFile { get; set; } = null!;
}