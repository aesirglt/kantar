using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands;
using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Queries;
using Kantar.TechnicalAssessment.ApplicationService.Interfaces;
using Kantar.TechnicalAssessment.ApplicationService.ViewModels;
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
                await _itemService.GetAllAsync([.. command.Items.ConvertAll(i => i.ItemId)], cancellationToken);

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
                : await CreateBasketItems(command, basketResult.ResultValue, maybeItems, cancellationToken);
        }

        public async Task<FSharpResult<BasketDetailViewModel, DomainError>> GetById(GetByIdBasketQuery query, CancellationToken cancellationToken)
        {
            return await _basketService.GetAsync(query.BasketId, cancellationToken)
                .ContinueWith(basketResultTask =>
                {
                    if (basketResultTask.Result.IsError)
                        return FSharpResult<BasketDetailViewModel, DomainError>.NewError(basketResultTask.Result.ErrorValue);

                    var basket = basketResultTask.Result.ResultValue;
                    var basketDetailViewModel = new BasketDetailViewModel
                    {
                        Id = basket.Id,
                        Items = [.. basket.BasketItems.Select(bi => new BasketItemViewModel
                        {
                            Id = bi.ItemId,
                            Quantity = bi.Quantity,
                            UnitPrice = bi.UnitPrice,
                            Name = bi.Item.Name,
                            Total = bi.Total,
                            SubTotal = bi.SubTotal,
                            Discounts = bi.Discounts
                        })],
                        Totals = new ()
                        {
                            SubTotal = basket.Subtotal,
                            Total = basket.Total,
                            Discount = basket.Discounts
                        }
                    };
                    return FSharpResult<BasketDetailViewModel, DomainError>.NewOk(basketDetailViewModel);
                }, cancellationToken);
        }
        private async Task<BasketResult> CreateBasketItems(CreateBasketCommand command, Basket basket,
            FSharpResult<IQueryable<Item>, DomainError> maybeItems, CancellationToken cancellationToken)
        {
            var basketItems =
                maybeItems.ResultValue.ToList().Select(item => new BasketItem
                {
                    BasketId = basket.Id,
                    ItemId = item.Id,
                    UnitPrice = item.Price,
                    Quantity = command.Items.First(x => x.ItemId == item.Id).Quantity,
                    CreatedAt = DateTime.UtcNow
                }).Where(x => !basket.BasketItems.Exists(c => c.ItemId == x.ItemId)).ToList();

            if(basketItems is { Count: 0 }) return BasketResult.NewOk(default!);

            var discountResult = await _discountService.GetByItemIdsAsync([.. basketItems.Select(x => x.ItemId)], cancellationToken);

            List<Discount> discounts = [];

            if (discountResult.IsOk)
            {
                discounts = [.. discountResult.ResultValue];
            }

            var basketWithDiscounts = _applyDiscountDomainService.ApplyDiscounts(basketItems, discounts).ToList();

            return await _basketItemService.CreateAsync([.. basketWithDiscounts], cancellationToken);
        }
    }
}
