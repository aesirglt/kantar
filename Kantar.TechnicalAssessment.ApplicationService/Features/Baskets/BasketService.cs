using Kantar.TechnicalAssessment.Domain;
using Kantar.TechnicalAssessment.Domain.Errors;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Microsoft.FSharp.Core;
using static Microsoft.FSharp.Core.FSharpResult<Kantar.TechnicalAssessment.Domain.Features.Basket,
    Kantar.TechnicalAssessment.Domain.DomainError>;

namespace Kantar.TechnicalAssessment.ApplicationService.Features.Baskets
{
    public class BasketService : IBasketService
    {
        private readonly IBaseRepository<Basket> _basketRepository;

        public BasketService(IBaseRepository<Basket> basketRepository)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        }

        public async Task<FSharpResult<Basket, DomainError>> CreateAsync(Basket basket, CancellationToken cancellationToken)
            => basket == null
                ? NewError(new InvalidObjectError("Basket cant be null."))
                : await _basketRepository.Add(basket, cancellationToken);

        public Task<FSharpResult<IQueryable<Basket>, DomainError>> GetAllAsync(CancellationToken cancellationToken)
            => _basketRepository.GetAll(cancellationToken);

        public async Task<FSharpResult<Basket, DomainError>> GetAsync(Guid basketId, CancellationToken cancellationToken)
            => basketId == Guid.Empty
            ? NewError(new InvalidObjectError("BasketId cant be default value."))
            : await _basketRepository.GetById(basketId, cancellationToken);

        public async Task<FSharpResult<Unit, DomainError>> RemoveAsync(Basket basket, CancellationToken cancellationToken)
         => basket == null
                ? FSharpResult<Unit, DomainError>.NewError(new InvalidObjectError("Basket cant be null."))
                : await _basketRepository.Delete(basket, cancellationToken);
    }
}
