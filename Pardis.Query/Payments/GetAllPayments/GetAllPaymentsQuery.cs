using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pardis.Query.Payments.GetAllPayments
{
    public record GetAllPaymentsQuery : IRequest<OperationResult<List<ManualPaymentRequestDto>>>;
}
