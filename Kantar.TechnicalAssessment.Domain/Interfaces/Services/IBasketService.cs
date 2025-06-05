using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.Services
{
    public interface IBasketService
    {
        Task<FSharpResult<Basket, DomainError>> CreateAsync(Basket basket, CancellationToken cancellationToken);
        Task<FSharpResult<Basket, DomainError>> GetAsync(Guid basketId, CancellationToken cancellationToken);
    }
}
