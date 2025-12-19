using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Accounting;
using Pardis.Domain.Dto.Accounting;
using Pardis.Domain.Users;
using Pardis.Query.Accounting;
using Api.Controllers;

namespace Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت حسابداری - مدیریت تراکنش‌ها و گزارش‌های مالی
/// </summary>
[Route("api/admin/accounting")]
[ApiController]
[Produces("application/json")]
[Tags("Accounting Management")]
[Authorize(Roles = Role.Admin + "," + Role.Manager + "," + "GeneralManager" + "," + "FinancialManager")]
public class AccountingController : BaseController
{
    private readonly IMediator _mediator;

    public AccountingController(IMediator mediator, ILogger<AccountingController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت آمار حسابداری
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        return await ExecuteAsync(async () =>
        {
            var stats = await _mediator.Send(new GetAccountingStatsQuery());
            return SuccessResponse(stats, "آمار حسابداری با موفقیت دریافت شد");
        }, "خطا در دریافت آمار حسابداری");
    }

    /// <summary>
    /// دریافت لیست تراکنش‌ها با فیلتر
    /// </summary>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] GetTransactionsQuery query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _mediator.Send(query);
            return Ok(new
            {
                success = true,
                message = "لیست تراکنش‌ها با موفقیت دریافت شد",
                data = result.Transactions,
                pagination = new
                {
                    currentPage = query.Page,
                    pageSize = query.PageSize,
                    totalCount = result.TotalCount,
                    totalPages = (int)Math.Ceiling((double)result.TotalCount / query.PageSize)
                }
            });
        }, "خطا در دریافت لیست تراکنش‌ها");
    }

    /// <summary>
    /// دریافت جزئیات یک تراکنش
    /// </summary>
    [HttpGet("transactions/{id}")]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            var transaction = await _mediator.Send(new GetTransactionByIdQuery(id));
            
            if (transaction == null)
                return NotFound(new { 
                    success = false, 
                    message = "تراکنش یافت نشد" 
                });

            return SuccessResponse(transaction, "جزئیات تراکنش با موفقیت دریافت شد");
        }, "خطا در دریافت جزئیات تراکنش");
    }

    /// <summary>
    /// بازگشت وجه تراکنش
    /// </summary>
    [HttpPost("transactions/{id}/refund")]
    public async Task<IActionResult> RefundTransaction(Guid id, [FromBody] RefundTransactionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { 
                    success = false, 
                    message = "کاربر احراز هویت نشده است" 
                });

            var command = new RefundTransactionCommand
            {
                TransactionId = id,
                Reason = dto.Reason,
                RefundAmount = dto.RefundAmount,
                AdminUserId = userId
            };

            var result = await _mediator.Send(command);
            return HandleOperationResult(result);
        }, "خطا در بازگشت وجه تراکنش");
    }

    /// <summary>
    /// ایجاد تراکنش جدید (برای تست یا ثبت دستی)
    /// </summary>
    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _mediator.Send(command);
            
            if (result.Status == Pardis.Application._Shared.OperationResultStatus.Success)
            {
                return StatusCode(201, new { 
                    success = true,
                    message = "تراکنش با موفقیت ایجاد شد", 
                    data = result.Data 
                });
            }

            return HandleOperationResult(result);
        }, "خطا در ایجاد تراکنش");
    }

    /// <summary>
    /// بروزرسانی وضعیت تراکنش
    /// </summary>
    [HttpPut("transactions/{id}/status")]
    public async Task<IActionResult> UpdateTransactionStatus(Guid id, [FromBody] UpdateTransactionStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var command = new UpdateTransactionStatusCommand
            {
                TransactionId = id,
                Status = dto.Status,
                GatewayTransactionId = dto.GatewayTransactionId,
                Description = dto.Description
            };
            
            var result = await _mediator.Send(command);
            return HandleOperationResult(result);
        }, "خطا در بروزرسانی وضعیت تراکنش");
    }
}