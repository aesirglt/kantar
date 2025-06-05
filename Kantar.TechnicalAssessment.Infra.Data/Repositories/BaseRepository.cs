using Kantar.TechnicalAssessment.Domain;
using Kantar.TechnicalAssessment.Domain.Errors;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Kantar.TechnicalAssessment.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Core;

namespace Kantar.TechnicalAssessment.Infra.Data.Repositories
{
    public class BaseRepository<TEntity>(GroceryShoppingContext dbContext, ILogger<BaseRepository<TEntity>> logger) : IBaseRepository<TEntity> where TEntity : EntityBase<TEntity>
    {
        private readonly GroceryShoppingContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        private readonly ILogger<BaseRepository<TEntity>> _logger = logger;

        public async Task<FSharpResult<TEntity, DomainError>> Add(TEntity entity, CancellationToken cancellationToken)
        {
            try
            {
                var existingEntity = _dbContext.Set<TEntity>().Find(entity.Id);

                await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return FSharpResult<TEntity, DomainError>.NewOk(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, message: "An error occurred while adding the {TypeName}: {Message}", typeof(TEntity).Name, ex.Message);
                return FSharpResult<TEntity, DomainError>.NewError(new InternalError());
            }
        }

        public async Task<FSharpResult<Unit, DomainError>> Add(List<TEntity> entities, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return FSharpResult<Unit, DomainError>.NewOk(default!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, message: "An error occurred while adding the {TypeName}: {Message}", typeof(TEntity).Name, ex.Message);
                return FSharpResult<Unit, DomainError>.NewError(new InternalError());
            }
        }

        public async Task<FSharpResult<Unit, DomainError>> Delete(TEntity entity, CancellationToken cancellationToken)
        {
            try
            {
                var entityEntry = _dbContext.Set<TEntity>().Entry(entity);
                entityEntry.State = EntityState.Deleted;

                await _dbContext.SaveChangesAsync(cancellationToken);
                return FSharpResult<Unit, DomainError>.NewOk(default!);
            }
            catch
            {
                return FSharpResult<Unit, DomainError>.NewError(new InternalError("An error occurred while delete entity."));
            }
        }

        public async Task<FSharpResult<IQueryable<TEntity>, DomainError>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                return await Task.Run(() => FSharpResult<IQueryable<TEntity>, DomainError>.NewOk(
                                            _dbContext.Set<TEntity>()
                                            .AsNoTracking().AsQueryable()));
            }
            catch
            {
                return FSharpResult<IQueryable<TEntity>, DomainError>.NewError(new InternalError("An error occurred while retrieving all entities."));
            }
        }

        public async Task<FSharpResult<TEntity, DomainError>> GetById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var entityEntry = await _dbContext.Set<TEntity>().FindAsync([id], cancellationToken: cancellationToken);
                return entityEntry is null
                    ? FSharpResult<TEntity, DomainError>.NewError(new NotFoundError($"Entity with ID {id} not found."))
                    : FSharpResult<TEntity, DomainError>.NewOk(entityEntry);
            }
            catch
            {
                return FSharpResult<TEntity, DomainError>.NewError(new InternalError($"An error occurred while retrieving by id: {id}."));
            }
        }

        public Task<FSharpResult<Unit, DomainError>> Update(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
