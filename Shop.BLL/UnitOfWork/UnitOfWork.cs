using Shop.BLL.Interfaces;
using Shop.BLL.Repositories;
using Shop.DAL.Context;
using Shop.DAL.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShopDbContext _dbContext;
        private readonly Hashtable _repositories;
        

        public UnitOfWork(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }


        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();

        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var repoName = typeof(TEntity).Name;

            if(!_repositories.ContainsKey(repoName))
            {
                var repoValue = new GenericRepository<TEntity, TKey>(_dbContext);
                _repositories.Add(repoName, repoValue);
            }

            return _repositories[repoName] as GenericRepository<TEntity, TKey>;
        }
    }
}
