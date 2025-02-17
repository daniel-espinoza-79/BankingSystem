using Domain.Entities.Abstracts;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRepository<TEntity, TKey> where TEntity : Registered<TKey>
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetByAsync(int threshold, int limit, Func<TEntity, bool>? predicate = null);
        Task <TEntity> FindOneAsync(Func<TEntity, bool> predicate);
        Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool>? predicate = null);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TKey id, TEntity entity);
        Task RemoveAsync(TKey id);
        Task<int> CountAsync(Func<TEntity, bool>? predicate = null);
    }
}