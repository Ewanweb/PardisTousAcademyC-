using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Categories.Delete
{
    public class DeleteCategoryCommand : IRequest<OperationResult>
    {
        public Guid Id { get; set; }
        public Guid? MigrateToId { get; set; } // اختیاری
    }
}
