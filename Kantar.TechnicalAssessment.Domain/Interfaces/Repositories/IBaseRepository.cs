using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase<TEntity>
    {
        Task<FSharpResult<TEntity, DomainError>> Add(TEntity entity, CancellationToken cancellationToken);
        Task<FSharpResult<Unit, DomainError>> Add(List<TEntity> entities, CancellationToken cancellationToken);
        Task<FSharpResult<Unit, DomainError>> Update(TEntity entity, CancellationToken cancellationToken);
        Task<FSharpResult<Unit, DomainError>> Delete(TEntity entity, CancellationToken cancellationToken);
        Task<FSharpResult<TEntity, DomainError>> GetById(Guid id, CancellationToken cancellationToken);
        Task<FSharpResult<IQueryable<TEntity>, DomainError>> GetAll(CancellationToken cancellationToken);
    }
}
