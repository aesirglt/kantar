using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.DomainServices
{
    public interface IApplyDiscountDomainService
    {
        FSharpOption<Basket> ApplyDiscountsToBasket(Basket basket, List<Discount> discounts);
    }
}
