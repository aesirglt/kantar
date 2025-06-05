using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands;
using BasketResult = Microsoft.FSharp.Core.FSharpResult<Microsoft.FSharp.Core.Unit,
    Kantar.TechnicalAssessment.Domain.DomainError>;

namespace Kantar.TechnicalAssessment.ApplicationService.Interfaces
{
    public interface IBasketManagmentService
    {
        Task<BasketResult> AddAsync(CreateBasketCommand command, CancellationToken cancellationToken);
    }
}
