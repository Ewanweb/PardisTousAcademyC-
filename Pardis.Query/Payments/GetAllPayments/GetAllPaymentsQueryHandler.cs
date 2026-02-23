using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.Pagination;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Payments.GetAllPayments;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, OperationResult<PagedResult<ManualPaymentRequestDto>>>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetAllPaymentsQueryHandler(IPaymentAttemptRepository paymentAttemptRepository, IMapper mapper, UserManager<User> userManager)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<OperationResult<PagedResult<ManualPaymentRequestDto>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _paymentAttemptRepository.GetAllPagedAsync(
                request.Pagination,
                request.Search,
                request.Status,
                cancellationToken);

            var dto = _mapper.Map<List<ManualPaymentRequestDto>>(pagedResult.Items);
            foreach (var item in dto)
            {
                User? admin = null;
                if (!string.IsNullOrEmpty(item.AdminReviewedBy))
                    admin = await _userManager.FindByIdAsync(item.AdminReviewedBy);

                item.AdminReviewerName = admin?.FullName;
                
                // استخراج نام دوره‌ها و اطلاعات کامل از CartSnapshot
                var paymentAttempt = pagedResult.Items.FirstOrDefault(p => p.Id == item.Id);
                if (paymentAttempt?.Order?.CartSnapshot != null)
                {
                    var courseItems = ExtractCourseItems(paymentAttempt.Order.CartSnapshot);
                    item.CourseItems = courseItems;
                    item.CourseNames = courseItems
                        .Where(c => !string.IsNullOrWhiteSpace(c.CourseTitle))
                        .Select(c => c.CourseTitle)
                        .ToList();
                }
            }

            var response = new PagedResult<ManualPaymentRequestDto>
            {
                Items = dto,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                HasNext = pagedResult.HasNext,
                HasPrev = pagedResult.HasPrev,
                Stats = pagedResult.Stats
            };

            return OperationResult<PagedResult<ManualPaymentRequestDto>>.Success(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return OperationResult<PagedResult<ManualPaymentRequestDto>>.Success(new PagedResult<ManualPaymentRequestDto>());
        }
    }
    
    private List<Pardis.Domain.Dto.Payments.CourseItemDto> ExtractCourseItems(string cartSnapshot)
    {
        try
        {
            if (string.IsNullOrEmpty(cartSnapshot))
                return new List<Pardis.Domain.Dto.Payments.CourseItemDto>();

            // CartSnapshot uses "Title" not "CourseTitle"
            var items = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(cartSnapshot);
            
            return items?.Select(item => new Pardis.Domain.Dto.Payments.CourseItemDto
            {
                CourseId = item.TryGetProperty("CourseId", out var courseId) ? courseId.GetGuid() : Guid.Empty,
                CourseTitle = item.TryGetProperty("Title", out var title) ? title.GetString() ?? "نامشخص" : "نامشخص",
                Price = item.TryGetProperty("Price", out var price) ? price.GetInt64() : 0
            }).ToList() ?? new List<Pardis.Domain.Dto.Payments.CourseItemDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting course items: {ex.Message}");
            return new List<Pardis.Domain.Dto.Payments.CourseItemDto>();
        }
    }
}

// Helper class for cart snapshot deserialization (not used anymore, using JsonElement instead)
public class CartItemSnapshot
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public long Price { get; set; }
}
