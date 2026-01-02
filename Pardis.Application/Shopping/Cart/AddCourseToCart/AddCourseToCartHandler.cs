using MediatR;
using AutoMapper;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.Courses.Contracts;
using Pardis.Application.Payments.Contracts;
using Pardis.Domain.Shopping;

namespace Pardis.Application.Shopping.Cart.AddCourseToCart;

/// <summary>
/// پردازشگر دستور اضافه کردن دوره به سبد خرید
/// </summary>
public class AddCourseToCartHandler : IRequestHandler<AddCourseToCartCommand, OperationResult<AddCourseToCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public AddCourseToCartHandler(
        ICartRepository cartRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<AddCourseToCartResult>> Handle(AddCourseToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // بررسی وجود دوره
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return OperationResult<AddCourseToCartResult>.NotFound("دوره یافت نشد");

            // بررسی اینکه کاربر قبلاً در این دوره ثبت‌نام کرده یا نه
            var existingEnrollment = await _enrollmentRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId, cancellationToken);
            if (existingEnrollment != null)
                return OperationResult<AddCourseToCartResult>.Error("شما قبلاً در این دوره ثبت‌نام کرده‌اید");

            // دریافت یا ایجاد سبد خرید کاربر
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart == null)
            {
                cart = new Domain.Shopping.Cart(request.UserId);
                cart = await _cartRepository.CreateAsync(cart, cancellationToken);
                await _cartRepository.SaveChangesAsync(cancellationToken); // ذخیره سبد جدید قبل از اضافه کردن آیتم
            }

            // بررسی اینکه دوره قبلاً در سبد وجود دارد یا نه
            if (cart.ContainsCourse(request.CourseId))
                return OperationResult<AddCourseToCartResult>.Error("این دوره قبلاً به سبد خرید اضافه شده است");

            // اضافه کردن دوره به سبد
            cart.AddCourse(course);
            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);

            // ایجاد نتیجه
            var result = new AddCourseToCartResult
            {
                CartId = cart.Id,
                CartItemId = cart.Items.First(i => i.CourseId == request.CourseId).Id,
                TotalItems = cart.GetItemCount(),
                TotalAmount = cart.TotalAmount,
                Message = "دوره با موفقیت به سبد خرید اضافه شد"
            };

            return OperationResult<AddCourseToCartResult>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<AddCourseToCartResult>.Error($"خطا در اضافه کردن دوره به سبد خرید: {ex.Message}");
        }
    }
}