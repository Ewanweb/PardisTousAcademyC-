using MediatR;
using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using AutoMapper;

namespace Pardis.Application.Payments.ManualPayment.UploadReceipt;

/// <summary>
/// Handler برای آپلود رسید پرداخت دستی
/// </summary>
public class UploadReceiptHandler : IRequestHandler<UploadReceiptCommand, OperationResult<ManualPaymentRequestDto>>
{
    private readonly IManualPaymentRequestRepository _manualPaymentRepository;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public UploadReceiptHandler(
        IManualPaymentRequestRepository manualPaymentRepository,
        IFileService fileService,
        IMapper mapper)
    {
        _manualPaymentRepository = manualPaymentRepository;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<OperationResult<ManualPaymentRequestDto>> Handle(UploadReceiptCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود درخواست پرداخت
        var paymentRequest = await _manualPaymentRepository.GetByIdWithDetailsAsync(request.PaymentRequestId, cancellationToken);
        if (paymentRequest == null)
            return OperationResult<ManualPaymentRequestDto>.Error("درخواست پرداخت یافت نشد");

        // بررسی دسترسی کاربر
        if (paymentRequest.StudentId != request.StudentId)
            return OperationResult<ManualPaymentRequestDto>.Error("دسترسی غیرمجاز");

        // بررسی وضعیت درخواست
        if (paymentRequest.Status != ManualPaymentStatus.PendingReceipt)
            return OperationResult<ManualPaymentRequestDto>.Error("فقط در وضعیت انتظار رسید می‌توان رسید آپلود کرد");

        // اعتبارسنجی فایل
        var validationResult = ValidateReceiptFile(request.ReceiptFile);
        if (!validationResult.IsValid)
            return OperationResult<ManualPaymentRequestDto>.Error(validationResult.ErrorMessage);

        try
        {
            // آپلود فایل با استفاده از IFileService
            var directoryPath = Path.Combine("uploads", "receipts");
            var fileName = await _fileService.SaveFileAndGenerateName(request.ReceiptFile, directoryPath);
            var fileUrl = $"/{directoryPath}/{fileName}";

            // به‌روزرسانی درخواست پرداخت
            paymentRequest.UploadReceipt(fileUrl, request.ReceiptFile.FileName);
            await _manualPaymentRepository.SaveChangesAsync(cancellationToken);

            // تبدیل به DTO با استفاده از AutoMapper
            var dto = _mapper.Map<ManualPaymentRequestDto>(paymentRequest);

            return OperationResult<ManualPaymentRequestDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return OperationResult<ManualPaymentRequestDto>.Error($"خطا در آپلود فایل: {ex.Message}");
        }
    }

    private static FileValidationResult ValidateReceiptFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return new FileValidationResult { IsValid = false, ErrorMessage = "فایل انتخاب نشده است" };

        // بررسی اندازه فایل (حداکثر 5 مگابایت)
        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
            return new FileValidationResult { IsValid = false, ErrorMessage = "حداکثر اندازه فایل 5 مگابایت است" };

        // بررسی نوع فایل
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            return new FileValidationResult { IsValid = false, ErrorMessage = "فقط فایل‌های JPG، PNG و PDF مجاز هستند" };

        // بررسی Content-Type
        var allowedContentTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!allowedContentTypes.Contains(file.ContentType))
            return new FileValidationResult { IsValid = false, ErrorMessage = "نوع فایل نامعتبر است" };

        return new FileValidationResult { IsValid = true };
    }
}

/// <summary>
/// نتیجه اعتبارسنجی فایل
/// </summary>
public class FileValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}