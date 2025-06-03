using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase<TEntity>
    {
        FSharpResult<TEntity, DomainError> Add(TEntity entity);
        FSharpResult<Unit, DomainError> Update(TEntity entity);
        FSharpResult<Unit, DomainError> Delete(TEntity entity);
        FSharpResult<TEntity, DomainError> GetById(int id);
        FSharpResult<IEnumerable<TEntity>, DomainError> GetAll();
    }
}
