using Kantar.TechnicalAssessment.Domain.Enums;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.DomainServices;
using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.DomainServices
{
    public class ApplyDiscountDomainService : IApplyDiscountDomainService
    {
        public FSharpOption<Basket> ApplyDiscountsToBasket(Basket basket, List<Discount> discounts)
        {
            if (basket == null || discounts is not { } or { Count: 0 })
                return FSharpOption<Basket>.None;

            return basket with
            {
                Discounts = GetDiscount(basket, discounts)
            };
        }

        private static decimal GetDiscount(Basket basket, List<Discount> discounts)
            => basket.BasketItems
                .SelectMany(basketItem =>
                    discounts
                    .Where(d => d.ItemId == basketItem.ItemId)
                    .Select(discount =>
                    {
                        var unitPrice = basketItem.Item.Price;
                        var totalPrice = unitPrice * basketItem.Quantity;

                        return discount.DiscountType switch
                        {
                            DiscountType.Percentage => unitPrice * (discount.Amount / 100m) * basketItem.Quantity,
                            DiscountType.Fixed => discount.Amount * basketItem.Quantity,
                            _ => 0m
                        };
                    }))
                    .Sum();
    }
}
