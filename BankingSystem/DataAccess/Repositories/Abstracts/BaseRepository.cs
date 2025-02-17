using Domain.Entities.Abstracts;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Abstracts
{
    public class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Registered<TKey>
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.Where(entity => entity.Id!.Equals(id) && entity.IsActive).FirstOrDefaultAsync();
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            entity.RegisteredAt = DateTime.UtcNow;
            entity.IsActive = true;
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TKey id, TEntity entity)
        {
            var existingEntity = await _dbSet.FindAsync(id);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await Task.CompletedTask;
        }


        public virtual async Task RemoveAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                entity.IsActive = false;
                _dbSet.Update(entity);
            }
            await _context.SaveChangesAsync();
            await Task.CompletedTask;
        }


        public async Task<IEnumerable<TEntity>> GetByAsync(int threshold, int limit, Func<TEntity, bool>? predicate = null)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate)
                .Where(entity => entity.IsActive)
                .AsQueryable();
            }

            var pagedResult = query.Skip(threshold - 1).Take(limit);

            return await Task.FromResult(pagedResult.ToList());
        }

        public async Task<int> CountAsync(Func<TEntity, bool>? predicate = null)
        {
            if (predicate != null)
            {
                return await Task.FromResult(_dbSet
                .Where(predicate).Where(entity => entity.IsActive).Count());
            }
            else
            {
                return await _context.Set<TEntity>().Where(entity => entity.IsActive).CountAsync();
            }

        }

        public async Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool>? predicate = null)
        {
            var query = _dbSet.AsQueryable();

            query = query.Where(entity => entity.IsActive);

            if (predicate != null)
            {
                query = query.Where(predicate)
                .AsQueryable();
            }
            return await Task.FromResult(query.ToList());
        }
        
        public async Task<TEntity?> FindOneAsync(Func<TEntity, bool> predicate)
        {
            var query = _dbSet.AsQueryable()
                .Where(entity => entity.IsActive)
                .Where(predicate);

            return await Task.FromResult(query.FirstOrDefault());
        }
    }
}