using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands;
using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Queries;
using Kantar.TechnicalAssessment.ApplicationService.ViewModels;
using Kantar.TechnicalAssessment.Domain;
using Microsoft.FSharp.Core;
using BasketResult = Microsoft.FSharp.Core.FSharpResult<Microsoft.FSharp.Core.Unit,
    Kantar.TechnicalAssessment.Domain.DomainError>;

namespace Kantar.TechnicalAssessment.ApplicationService.Interfaces
{
    public interface IBasketManagmentService
    {
        Task<BasketResult> AddAsync(CreateBasketCommand command, CancellationToken cancellationToken);
        Task<BasketResult> UpdateItemQuantityAsync(UpdateBasketItemQuantityCommand command, CancellationToken cancellationToken);
        Task<BasketResult> RemoveAsync(DeleteBasketCommand command, CancellationToken cancellationToken);
        Task<BasketResult> RemoveBasketItemAsync(RemoveBasketItemCommand command, CancellationToken cancellationToken);
        Task<FSharpResult<BasketDetailViewModel, DomainError>> GetById(GetByIdBasketQuery query, CancellationToken cancellationToken);
        Task<FSharpResult<IQueryable<BasketResumeViewModel>, DomainError>> GetAll(GetAllBasketQuery query, CancellationToken cancellationToken);
    }
}
