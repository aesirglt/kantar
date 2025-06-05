using Kantar.TechnicalAssessment.Domain.Features;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.DomainServices
{
    public interface IApplyDiscountDomainService
    {
        IEnumerable<BasketItem> ApplyDiscounts(
            IEnumerable<BasketItem> basketItems,
            List<Discount> discounts);
    }
}
