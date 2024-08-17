using Microsoft.EntityFrameworkCore;
using Shop.BLL.Interfaces;
using Shop.BLL.Specifications;
using Shop.DAL.Context;
using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly ShopDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
            => await _dbContext.AddAsync(entity);

        public void Delete(TEntity entity)
            => _dbContext.Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecification<TEntity, TKey> spec)
            => await ApplySpec<TKey>(spec).ToListAsync();

        public async Task<TEntity> GetByIdAsync(TKey id)
            => await _dbSet.FindAsync(id);

        public async Task<TEntity> GetByIdWithSpecAsync(ISpecification<TEntity, TKey> spec)
            => await ApplySpec<TKey>(spec).FirstOrDefaultAsync();

        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public IQueryable<TEntity> ApplySpec<Tkey>(ISpecification<TEntity, TKey> spec)
            => SpecificationEvaluator<TEntity, TKey>.GetQuery(_dbSet, spec);

        public void DeleteRange(IEnumerable<TEntity> entities)
            => _dbSet.RemoveRange(entities);
	}
}
