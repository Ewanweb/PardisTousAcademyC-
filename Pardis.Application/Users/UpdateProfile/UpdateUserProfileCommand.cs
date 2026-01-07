using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;
using System.ComponentModel.DataAnnotations;

namespace Pardis.Application.Users.UpdateProfile;

public partial class UpdateUserProfileCommand : IRequest<OperationResult<UserProfileDto>>
{
    [StringLength(200, ErrorMessage = "نام کامل نمی‌تواند بیش از 200 کاراکتر باشد.")]
    public string? FullName { get; set; }

    [StringLength(500, ErrorMessage = "بیوگرافی نمی‌تواند بیش از 500 کاراکتر باشد.")]
    public string? Bio { get; set; }

    public DateTime? BirthDate { get; set; }

    public Gender? Gender { get; set; }

    [StringLength(500, ErrorMessage = "آدرس نمی‌تواند بیش از 500 کاراکتر باشد.")]
    public string? Address { get; set; }

    [Phone(ErrorMessage = "شماره موبایل معتبر نیست.")]
    [StringLength(15, ErrorMessage = "شماره موبایل نمی‌تواند بیش از 15 کاراکتر باشد.")]
    public string? PhoneNumber { get; set; }

    public string? UserId { get; set; }
}