using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.Services
{
    public interface IBasketItemService
    {
        Task<FSharpResult<Unit, DomainError>> RemoveAsync(Guid basketItemId, CancellationToken cancellationToken);
        Task<FSharpResult<Unit, DomainError>> UpdateAsync(List<BasketItem> basketItems, CancellationToken cancellationToken);
        Task<FSharpResult<Unit, DomainError>> CreateAsync(List<BasketItem> basketItems, CancellationToken cancellationToken);
    }
}
