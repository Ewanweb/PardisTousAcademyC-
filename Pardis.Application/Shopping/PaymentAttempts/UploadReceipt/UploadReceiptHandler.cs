using MediatR;
using AutoMapper;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.FileUtil;
using Pardis.Domain.Shopping;

namespace Pardis.Application.Shopping.PaymentAttempts.UploadReceipt;

/// <summary>
/// پردازشگر دستور آپلود رسید پرداخت
/// </summary>
public class UploadReceiptHandler : IRequestHandler<UploadReceiptCommand, OperationResult<UploadReceiptResult>>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public UploadReceiptHandler(
        IPaymentAttemptRepository paymentAttemptRepository,
        IFileService fileService,
        IMapper mapper)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<OperationResult<UploadReceiptResult>> Handle(UploadReceiptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // دریافت تلاش پرداخت
            var paymentAttempt = await _paymentAttemptRepository.GetByIdAsync(request.PaymentAttemptId, cancellationToken);
            if (paymentAttempt == null)
                return OperationResult<UploadReceiptResult>.NotFound("تلاش پرداخت یافت نشد");

            // بررسی مالکیت
            if (paymentAttempt.UserId != request.UserId)
                return OperationResult<UploadReceiptResult>.Error("شما مجاز به انجام این عملیات نیستید");

            // بررسی روش پرداخت
            if (paymentAttempt.Method != PaymentMethod.Manual)
                return OperationResult<UploadReceiptResult>.Error("فقط پرداخت‌های دستی می‌توانند رسید داشته باشند");

            // بررسی وضعیت
            if (!paymentAttempt.RequiresReceiptUpload() && paymentAttempt.Status != PaymentAttemptStatus.AwaitingAdminApproval)
                return OperationResult<UploadReceiptResult>.Error("وضعیت پرداخت برای آپلود رسید مناسب نیست");

            // اعتبارسنجی فایل
            if (request.ReceiptFile == null || request.ReceiptFile.Length == 0)
                return OperationResult<UploadReceiptResult>.Error("فایل رسید ارسال نشده است");

            if (request.ReceiptFile.Length > 5 * 1024 * 1024)
                return OperationResult<UploadReceiptResult>.Error("حداکثر حجم فایل رسید ۵ مگابایت است");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
            var extension = System.IO.Path.GetExtension(request.ReceiptFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return OperationResult<UploadReceiptResult>.Error("فرمت فایل نامعتبر است. فقط تصاویر و PDF مجاز هستند");

            // آپلود فایل - save to wwwroot/uploads/payment-receipts
            var fileName = await _fileService.SaveFileAndGenerateName(request.ReceiptFile, "uploads/payment-receipts");
            if (string.IsNullOrEmpty(fileName))
                return OperationResult<UploadReceiptResult>.Error("خطا در آپلود فایل");

            var fileUrl = $"/uploads/payment-receipts/{fileName}";

            // به‌روزرسانی تلاش پرداخت
            paymentAttempt.UploadReceipt(fileUrl, fileName);
            await _paymentAttemptRepository.UpdateAsync(paymentAttempt, cancellationToken);
            await _paymentAttemptRepository.SaveChangesAsync(cancellationToken);

            // ایجاد نتیجه
            var result = new UploadReceiptResult
            {
                PaymentAttemptId = paymentAttempt.Id,
                TrackingCode = paymentAttempt.TrackingCode ?? string.Empty,
                ReceiptUrl = fileUrl,
                ReceiptFileName = fileName,
                Amount = paymentAttempt.Amount,
                Status = (int)paymentAttempt.Status,
                RejectReason = paymentAttempt.FailureReason,
                UploadedAt = paymentAttempt.ReceiptUploadedAt ?? DateTime.UtcNow,
                Message = "رسید با موفقیت آپلود شد و در انتظار تایید ادمین است"
            };

            return OperationResult<UploadReceiptResult>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<UploadReceiptResult>.Error($"خطا در آپلود رسید: {ex.Message}");
        }
    }
}