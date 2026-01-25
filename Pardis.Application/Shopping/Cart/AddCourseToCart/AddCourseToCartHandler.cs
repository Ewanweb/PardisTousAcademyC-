using MediatR;
using AutoMapper;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.Shopping.Validation;
using Microsoft.Extensions.Logging;

namespace Pardis.Application.Shopping.Cart.AddCourseToCart;

/// <summary>
/// پردازشگر دستور اضافه کردن دوره به سبد خرید - بهبود یافته با اعتبارسنجی جامع
/// </summary>
public class AddCourseToCartHandler : IRequestHandler<AddCourseToCartCommand, OperationResult<AddCourseToCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartValidationService _validationService;
    private readonly ILogger<AddCourseToCartHandler> _logger;
    private readonly IMapper _mapper;

    public AddCourseToCartHandler(
        ICartRepository cartRepository,
        ICartValidationService validationService,
        ILogger<AddCourseToCartHandler> logger,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _validationService = validationService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<OperationResult<AddCourseToCartResult>> Handle(AddCourseToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Adding course {CourseId} to cart for user {UserId}", request.CourseId, request.UserId);

            // اعتبارسنجی جامع
            var validationResult = await _validationService.ValidateAddCourseToCartAsync(request.UserId, request.CourseId, cancellationToken);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First();
                _logger.LogWarning("Validation failed for adding course {CourseId} to cart: {ErrorCode} - {ErrorMessage}", 
                    request.CourseId, firstError.Code, firstError.Message);
                
                return firstError.Code switch
                {
                    "COURSE_NOT_FOUND" => OperationResult<AddCourseToCartResult>.NotFound(firstError.Message),
                    "ALREADY_ENROLLED" => OperationResult<AddCourseToCartResult>.Error(firstError.Message),
                    "ALREADY_IN_CART" => OperationResult<AddCourseToCartResult>.Error(firstError.Message),
                    "COURSE_FULL" => OperationResult<AddCourseToCartResult>.Error(firstError.Message),
                    "PREREQUISITE_NOT_MET" => OperationResult<AddCourseToCartResult>.Error(firstError.Message),
                    _ => OperationResult<AddCourseToCartResult>.Error(firstError.Message)
                };
            }

            var course = validationResult.Course!;

            // دریافت یا ایجاد سبد خرید کاربر
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart == null)
            {
                _logger.LogInformation("Creating new cart for user {UserId}", request.UserId);
                cart = new Domain.Shopping.Cart(request.UserId);
                cart = await _cartRepository.CreateAsync(cart, cancellationToken);
                await _cartRepository.SaveChangesAsync(cancellationToken); // ذخیره سبد جدید قبل از اضافه کردن آیتم
            }

            // اضافه کردن دوره به سبد با snapshot کامل
            cart.AddCourse(course);

            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully added course {CourseId} to cart {CartId} for user {UserId}. Cart now has {ItemCount} items", 
                request.CourseId, cart.Id, request.UserId, cart.GetItemCount());

            // دریافت آیتم اضافه شده برای نتیجه
            var addedItem = cart.Items.First(i => i.CourseId == request.CourseId);

            // ایجاد نتیجه با اطلاعات کامل
            var result = new AddCourseToCartResult
            {
                CartId = cart.Id,
                CartItemId = addedItem.Id,
                TotalItems = cart.GetItemCount(),
                TotalAmount = cart.TotalAmount,
                CourseTitle = course.Title,
                CoursePrice = course.Price,
                Message = "دوره با موفقیت به سبد خرید اضافه شد",
                Warnings = validationResult.Warnings.Select(w => w.Message).ToList()
            };

            return OperationResult<AddCourseToCartResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding course {CourseId} to cart for user {UserId}", request.CourseId, request.UserId);
            return OperationResult<AddCourseToCartResult>.Error($"خطا در اضافه کردن دوره به سبد خرید: {ex.Message}");
        }
    }
}