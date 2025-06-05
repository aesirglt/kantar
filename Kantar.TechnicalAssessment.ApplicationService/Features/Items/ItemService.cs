using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Result = Microsoft.FSharp.Core.FSharpResult<
    System.Linq.IQueryable<Kantar.TechnicalAssessment.Domain.Features.Item>,
    Kantar.TechnicalAssessment.Domain.DomainError>;

namespace Kantar.TechnicalAssessment.ApplicationService.Features.Items
{
    public class ItemService(IBaseRepository<Item> itemRepository) : IItemService
    {
        private readonly IBaseRepository<Item> _itemRepository = itemRepository;
        
        public async Task<Result> GetAllAsync(List<Guid> itemIds, CancellationToken cancellationToken)
        {
            var allQueryable = await _itemRepository.GetAll(cancellationToken);

            if (allQueryable.IsError)
                return allQueryable;

            return Result.NewOk(allQueryable.ResultValue.Where(d => itemIds.Contains(d.Id)));
        }
    }
}
