using Kantar.TechnicalAssessment.Domain;
using Kantar.TechnicalAssessment.Domain.Errors;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Infra.Data.Repositories
{
    public class BaseRepository<TEntity>(DbContext dbContext, ILogger<BaseRepository<TEntity>> logger) : IBaseRepository<TEntity> where TEntity : EntityBase<TEntity>
    {
        private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        private readonly ILogger<BaseRepository<TEntity>> _logger = logger;

        public FSharpResult<TEntity, DomainError> Add(TEntity entity)
        {
            try
            {
                var existingEntity = _dbContext.Set<TEntity>().Find(entity.Id);

                _dbContext.Set<TEntity>().Add(entity);
                _dbContext.SaveChanges();

                return FSharpResult<TEntity, DomainError>.NewOk(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, message: "An error occurred while adding the {TypeName}: {Message}", typeof(TEntity).Name, ex.Message);
                return FSharpResult<TEntity, DomainError>.NewError(new InternalError());
            }
        }

        public FSharpResult<Unit, DomainError> Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public FSharpResult<IEnumerable<TEntity>, DomainError> GetAll()
        {
            throw new NotImplementedException();
        }

        public FSharpResult<TEntity, DomainError> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public FSharpResult<Unit, DomainError> Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
