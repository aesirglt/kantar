using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands;
using Kantar.TechnicalAssessment.ApplicationService.Interfaces;
using Kantar.TechnicalAssessment.Domain;
using Kantar.TechnicalAssessment.Domain.Errors;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.DomainServices;
using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Microsoft.FSharp.Core;
using BasketResult = Microsoft.FSharp.Core.FSharpResult<Microsoft.FSharp.Core.Unit,
    Kantar.TechnicalAssessment.Domain.DomainError>;

namespace Kantar.TechnicalAssessment.ApplicationService.Features.Managements
{
    public class BasketManagmentService(
        IBasketService basketService,
        IItemService itemService,
        IBasketItemService basketItemService,
        IDiscountService discountService,
        IApplyDiscountDomainService applyDiscountDomainService) : IBasketManagmentService
    {
        private readonly IBasketService _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        private readonly IItemService _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        private readonly IBasketItemService _basketItemService = basketItemService ?? throw new ArgumentNullException(nameof(basketItemService));
        private readonly IDiscountService _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        private readonly IApplyDiscountDomainService _applyDiscountDomainService = applyDiscountDomainService ?? throw new ArgumentNullException(nameof(applyDiscountDomainService));
        
        public async Task<BasketResult> AddAsync(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                return BasketResult.NewError(new InvalidObjectError("CreateBasketCommand cant be null."));

            var maybeItems =
                await _itemService.GetAllAsync([.. command.Items.Select(i => i.ItemId)], cancellationToken);

            if (maybeItems is { IsError: true })
                return BasketResult.NewError(maybeItems.ErrorValue);

            var basketInDbResult = await _basketService.GetAsync(command.BasketId, cancellationToken);

            var basketResult = (basketInDbResult.IsOk, basketInDbResult.ErrorValue) switch
            {
                (false, NotFoundError) => await _basketService.CreateAsync(new()
                {
                    Id = command.BasketId,
                    CreatedAt = DateTime.UtcNow,
                }, cancellationToken),
                (_, _) => basketInDbResult
            };

            return basketResult.IsError
                ? BasketResult.NewError(basketResult.ErrorValue)
                : await CreateBasketItems(command, basketResult.ResultValue, maybeItems,  cancellationToken);
        }

        private async Task<BasketResult> CreateBasketItems(CreateBasketCommand command, Basket basket,
            FSharpResult<IQueryable<Item>, DomainError> maybeItems, CancellationToken cancellationToken)
        {
            var basketItems =
                maybeItems.ResultValue.Select(item => new BasketItem
                {
                    BasketId = basket.Id,
                    ItemId = item.Id,
                    UnitPrice = item.Price,
                    Quantity = command.Items.First(x => x.ItemId == item.Id).Quantity,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

            var discountResult = await _discountService.GetByItemIdsAsync([.. basketItems.Select(x => x.ItemId)], cancellationToken);
            
            List<Discount> discounts = [];

            if (discountResult.IsOk)
            {
                discounts = [.. discountResult.ResultValue];
            }

            var basketWithDiscounts = _applyDiscountDomainService.ApplyDiscounts(basket.BasketItems, discounts);

            return await _basketItemService.CreateAsync([.. basketWithDiscounts], cancellationToken);
        }
    }
}
