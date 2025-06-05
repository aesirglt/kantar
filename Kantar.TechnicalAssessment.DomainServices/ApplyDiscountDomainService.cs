using Kantar.TechnicalAssessment.Domain.Enums;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.DomainServices;

namespace Kantar.TechnicalAssessment.DomainServices
{
    public class ApplyDiscountDomainService : IApplyDiscountDomainService
    {
        public IEnumerable<BasketItem> ApplyDiscounts(
            IEnumerable<BasketItem> basketItems,
            List<Discount> discounts)
        {
            if (discounts is null or { Count: 0 })
                return basketItems ?? [];

            return BasketItemsWithDiscounts(basketItems ?? [], discounts);
        }

        private static IEnumerable<BasketItem> BasketItemsWithDiscounts(
            IEnumerable<BasketItem> basketItems,
            List<Discount> discounts)
            => basketItems
                .SelectMany(basketItem => discounts
                    .Where(d => d.ItemId == basketItem.ItemId)
                    .Select(discount => basketItem with
                    {
                        Discounts = discount.DiscountType switch
                        {
                            DiscountType.Percentage => basketItem.UnitPrice * (discount.Value / 100m) * basketItem.Quantity,
                            DiscountType.Fixed => discount.Value * basketItem.Quantity,
                            DiscountType.ConditionalDivision => ConditionalDisivion(basketItems, basketItem, discount),
                            _ => 0m
                        },
                    }));

        private static decimal ConditionalDisivion(IEnumerable<BasketItem> basketItems, BasketItem basketItem, Discount discount)
        {
            long targetQuantity =
                basketItems
                .FirstOrDefault(x => x.ItemId == discount.ItemConditionalId)?.Quantity ?? 0;

            if (targetQuantity == 0)
                return 0;

            decimal applicableDiscounts = targetQuantity / discount.ConditionalQuantity;

            return (basketItem.UnitPrice / discount.Value) * Math.Min(basketItem.Quantity, applicableDiscounts);
        }
    }
}
