using Common.Dto.Paging.CursorPaging;
using Common.Exceptions;
using DAL.Extensions;
using Domain.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories.Abstract.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly BlogDbContext _db;

        protected BaseRepository(BlogDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct)
        {
            return await _db.Set<TEntity>().AnyAsync(e => e.Id == id, ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _db.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return await _db.Set<TEntity>().Where(predicate).OrderByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task RemoveAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);

            if (entity == null)
            {
                throw new ValidationException($"{typeof(TEntity).Name} of ID: {id} does not exist");
            }
             
            _db.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken)
        {
            _db.Entry(entity).State = EntityState.Modified;

            await _db.SaveChangesAsync(cancellationToken);

            return entity;
        }

        /// <summary>
        /// This method is used to return data from webApi, so it is called as no tracking comparing to GetById
        /// </summary>
        public async Task<TEntity?> GetByIdWithIncludeAsync(int id, CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = IncludeProperties(includeProperties);

            return await query.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _db.Set<TEntity>().AddAsync(entity, cancellationToken);

            return entity;
        }

        public async Task<CursorPagedResult<TEntity>> GetCursorPagedData(CursorPagedRequest pagedRequest,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = IncludeProperties(includeProperties);

            return await query.AsNoTracking().CreateCursorPagedResultAsync(pagedRequest, cancellationToken);
        }

        private IQueryable<TEntity> IncludeProperties(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> entities = _db.Set<TEntity>();

            foreach (var includeProperty in includeProperties)
            {
                entities = entities.Include(includeProperty);
            }

            return entities;
        }
    }
}