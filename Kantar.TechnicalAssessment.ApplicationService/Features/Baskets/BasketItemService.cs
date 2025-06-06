using Kantar.TechnicalAssessment.Domain;
using Kantar.TechnicalAssessment.Domain.Errors;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Microsoft.FSharp.Core;

using static Microsoft.FSharp.Core.FSharpResult<Microsoft.FSharp.Core.Unit,
    Kantar.TechnicalAssessment.Domain.DomainError>;
namespace Kantar.TechnicalAssessment.ApplicationService.Features.Baskets
{
    public class BasketItemService : IBasketItemService
    {
        private readonly IBaseRepository<BasketItem> _basketItemRepository;

        public BasketItemService(IBaseRepository<BasketItem> basketItemRepository)
        {
            _basketItemRepository = basketItemRepository ?? throw new ArgumentNullException(nameof(basketItemRepository));
        }

        public async Task<FSharpResult<Unit, DomainError>> CreateAsync(List<BasketItem> basketItems, CancellationToken cancellationToken)
        {
            if (basketItems is null or { Count: 0 })
                return NewError(new InvalidObjectError("Basket cant be null."));

            return await _basketItemRepository.Add(basketItems!, cancellationToken);
        }

        public async Task<FSharpResult<Unit, DomainError>> RemoveAsync(Guid basketItemId, CancellationToken cancellationToken)
        {
            var basket = await _basketItemRepository.GetById(basketItemId, cancellationToken);

            if(basket.IsError)
                return NewError(basket.ErrorValue);

            return await _basketItemRepository.Delete(basket.ResultValue, cancellationToken);
        }

        public async Task<FSharpResult<Unit, DomainError>> UpdateAsync(List<BasketItem> basketItems, CancellationToken cancellationToken)
        {
            if (basketItems is null or { Count: 0 })
                return NewError(new InvalidObjectError("Basket cant be null."));

            foreach (var item in basketItems)
            {
                if (item.Quantity <= 0)
                    return NewError(new InvalidObjectError("BasketItem quantity must be greater than zero."));

                await _basketItemRepository.Update(item!, cancellationToken);
            }


            return NewOk(default!);
        }
    }
}
