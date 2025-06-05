using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.Services
{
    public interface IDiscountService
    {
        Task<FSharpResult<IQueryable<Discount>, DomainError>> GetByItemIdsAsync(List<Guid> itemIds, CancellationToken cancellationToken);
    }
}
