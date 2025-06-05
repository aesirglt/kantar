using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.Services
{
    public interface IItemService
    {
        Task<FSharpResult<IQueryable<Item>, DomainError>> GetAllAsync(List<Guid> itemIds, CancellationToken cancellationToken);
    }
}
