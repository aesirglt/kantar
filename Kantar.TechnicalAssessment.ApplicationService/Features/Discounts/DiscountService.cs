using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Result = Microsoft.FSharp.Core.FSharpResult<
    System.Linq.IQueryable<Kantar.TechnicalAssessment.Domain.Features.Discount>,
    Kantar.TechnicalAssessment.Domain.DomainError>;

namespace Kantar.TechnicalAssessment.ApplicationService.Features.Discounts
{
    public class DiscountService(IBaseRepository<Discount> discountRepository) : IDiscountService
    {
        private readonly IBaseRepository<Discount> _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));

        public async Task<Result> GetByItemIdsAsync(List<Guid> itemIds, CancellationToken cancellationToken)
        {
            var allQueryable = await _discountRepository.GetAll(cancellationToken);

            if (allQueryable.IsError)
                return allQueryable;

            return Result.NewOk(allQueryable.ResultValue.Where(d => itemIds.Contains(d.ItemId)));
        }
    }
}
