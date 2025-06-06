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

        public async Task<FSharpResult<IQueryable<BasketResumeViewModel>, DomainError>> GetAll(GetAllBasketQuery query, CancellationToken cancellationToken)
        {
            var allQuery = await _basketService.GetAllAsync(cancellationToken);
            if (allQuery.IsError)
                return FSharpResult<IQueryable<BasketResumeViewModel>, DomainError>.NewError(allQuery.ErrorValue);

            var baskets = allQuery.ResultValue.Skip(query.Skip).Take(query.Take).ToList();

            var basketResumeViewModels =
                baskets.Select(b => new BasketResumeViewModel
                {
                    Id = b.Id,
                    Discounts = b.Discounts,
                    Total = b.Total,
                    Subtotal = b.Subtotal,
                    CreatedAt = b.CreatedAt,
                }).AsQueryable();

            return FSharpResult<IQueryable<BasketResumeViewModel>, DomainError>
                .NewOk(basketResumeViewModels);
        }

        public async Task<FSharpResult<BasketDetailViewModel, DomainError>> GetById(GetByIdBasketQuery query, CancellationToken cancellationToken)
            => await _basketService.GetAsync(query.BasketId, cancellationToken)
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
                            Id = bi.Id,
                            ItemId = bi.ItemId,
                            Quantity = bi.Quantity,
                            UnitPrice = bi.UnitPrice,
                            Name = bi.Item.Name,
                            Total = bi.Total,
                            SubTotal = bi.SubTotal,
                            Discounts = bi.Discounts
                        })],
                        Totals = new()
                        {
                            SubTotal = basket.Subtotal,
                            Total = basket.Total,
                            Discount = basket.Discounts
                        }
                    };
                    return FSharpResult<BasketDetailViewModel, DomainError>.NewOk(basketDetailViewModel);
                }, cancellationToken);

        public async Task<BasketResult> RemoveAsync(DeleteBasketCommand command, CancellationToken cancellationToken)
        {
            var basketResult = await _basketService.GetAsync(command.BasketId, cancellationToken);

            if (basketResult.IsError)
                return BasketResult.NewError(basketResult.ErrorValue);

            foreach (var item in basketResult.ResultValue.BasketItems)
            {
                await _basketItemService.RemoveAsync(item.Id, cancellationToken);
            }

            return await _basketService.RemoveAsync(basketResult.ResultValue with
            {
                BasketItems = []
            }, cancellationToken);
        }

        public async Task<BasketResult> RemoveBasketItemAsync(RemoveBasketItemCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                return BasketResult.NewError(new InvalidObjectError("RemoveBasketItemCommand cant be null."));

            var basket = await _basketService.GetAsync(command.BasketId, cancellationToken);

            if (basket.IsError)
                return BasketResult.NewError(basket.ErrorValue);

            if (basket.ResultValue.BasketItems.Exists(x => x.ItemId == command.ItemId) is false)
                return BasketResult.NewError(new NotFoundError($"BasketItem with id {command.ItemId} not found in basket {command.BasketId}."));

            foreach (var item in basket.ResultValue.BasketItems)
            {
                await _basketItemService.RemoveAsync(item.Id, cancellationToken);
            }

            var currentBasketItems =
                basket.ResultValue
                .BasketItems
                .Where(x => x.ItemId != command.ItemId)
                .Select(s => s with
                {
                    Id = Guid.Empty,
                    Basket = null!,
                    Item = null!,
                });

            var discountResult = await _discountService.GetByItemIdsAsync([.. currentBasketItems.Select(x => x.ItemId)], cancellationToken);

            List<Discount> discounts = [];

            if (discountResult.IsOk)
            {
                discounts = [.. discountResult.ResultValue];
            }

            var basketWithDiscounts = _applyDiscountDomainService.ApplyDiscounts(currentBasketItems, discounts).ToList();

            return await _basketItemService.CreateAsync([.. basketWithDiscounts], cancellationToken);
        }

        public async Task<BasketResult> UpdateItemQuantityAsync(UpdateBasketItemQuantityCommand command, CancellationToken cancellationToken)
        {
            var basketResult = await _basketService.GetAsync(command.BasketId, cancellationToken);

            if (basketResult.IsError)
                return BasketResult.NewError(basketResult.ErrorValue);

            var item = basketResult.ResultValue.BasketItems.FirstOrDefault(x => x.ItemId == command.ItemId);

            if (item is null)
                return BasketResult.NewError(new NotFoundError($"Item with id {command.ItemId} not found in basket {command.BasketId}."));

            var basketItems = basketResult.ResultValue.BasketItems;

            foreach (var basketItem in basketItems)
            {
                if(basketItem.ItemId == command.ItemId)
                {
                    basketItem.Quantity = command.Quantity;
                }
            }

            var discountResult = await _discountService.GetByItemIdsAsync([.. basketItems.Select(x => x.ItemId)], cancellationToken);
            List<Discount> discounts = [];
            if (discountResult.IsOk)
            {
                discounts = [.. discountResult.ResultValue];
            }

            var basketWithDiscounts = _applyDiscountDomainService.ApplyDiscounts(basketItems, discounts).ToList();

            return await _basketItemService.UpdateAsync(basketItems, cancellationToken);
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

            if (basketItems is { Count: 0 }) return BasketResult.NewOk(default!);

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
