using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories
{
    public interface IGenericRepository<T, TId> where T : class where TId : struct
    {
        IQueryable<T> GetAll();
        IQueryable<T> Where(Expression<Func<T,bool>> predicate);
        ValueTask<T?> GetByIdAsync(int id);
        Task<bool> AnyAsync(TId id);
        ValueTask AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
