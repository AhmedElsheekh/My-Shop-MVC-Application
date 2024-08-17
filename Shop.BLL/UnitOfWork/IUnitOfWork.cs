using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Shop.BLL.Interfaces;
using Shop.BLL.Repositories;
using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<int> CompleteAsync();
        IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
        
    }
}
