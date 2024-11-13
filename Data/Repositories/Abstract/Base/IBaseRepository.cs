using Common.Dto.Paging.CursorPaging;
using Domain.Abstract;
using System.Linq.Expressions;

namespace DAL.Repositories.Abstract.Base
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<bool> ExistsAsync(int id, CancellationToken ct);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<TEntity?> GetByIdWithIncludeAsync(int id, CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        Task<CursorPagedResult<TEntity>> GetCursorPagedData(CursorPagedRequest pagedRequest,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includeProperties);

        [Obsolete]
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> AddAsync(TEntity entity, bool saveChanges = false, CancellationToken cancellationToken = default);

        Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// This method is async only to retrieve entity of corresponding id asynchronously to ensure it is created
        /// </summary>
        Task RemoveAsync(int id, CancellationToken cancellationToken);
    }
}