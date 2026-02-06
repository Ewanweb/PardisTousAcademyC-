using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Consultation;

namespace Pardis.Application.Consultation;

public class CreateConsultationRequestHandler : IRequestHandler<CreateConsultationRequestCommand, OperationResult<Guid>>
{
    private readonly IConsultationRequestRepository _repository;

    public CreateConsultationRequestHandler(IConsultationRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResult<Guid>> Handle(CreateConsultationRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return OperationResult<Guid>.Error("نام و نام خانوادگی الزامی است");
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return OperationResult<Guid>.Error("شماره تماس الزامی است");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return OperationResult<Guid>.Error("پیام الزامی است");
            }

            // Create consultation request
            var consultationRequest = new ConsultationRequest
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Email = request.Email?.Trim(),
                CourseId = request.CourseId,
                CourseName = request.CourseName?.Trim(),
                Message = request.Message.Trim(),
                Status = ConsultationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UserId = request.UserId
            };

            await _repository.AddAsync(consultationRequest);

            var result = OperationResult<Guid>.Success(consultationRequest.Id);
            result.Message = "درخواست مشاوره با موفقیت ثبت شد. کارشناسان ما به زودی با شما تماس خواهند گرفت";
            return result;
        }
        catch (Exception ex)
        {
            return OperationResult<Guid>.Error($"خطا در ثبت درخواست مشاوره: {ex.Message}");
        }
    }
}
