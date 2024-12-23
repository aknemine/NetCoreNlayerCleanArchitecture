using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories
{
    public class GenericRepository<T,TId>(AppDbContext context) : IGenericRepository<T,TId> where T : BaseEntity<TId> where TId : struct
    {
        protected AppDbContext Context = context;//miras alınan sınıflarda kullanılsın diye protected
        private readonly DbSet<T> _dbSet=context.Set<T>();

        public async ValueTask AddAsync(T entity)=> await _dbSet.AddAsync(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);        

        public IQueryable<T> GetAll() => _dbSet.AsQueryable().AsNoTracking();

        public ValueTask<T?> GetByIdAsync(int id) => _dbSet.FindAsync(id);        

        public void Update(T entity)=> _dbSet.Update(entity);        

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).AsNoTracking();

        public Task<bool> AnyAsync(TId id) => _dbSet.AnyAsync(x => x.Id.Equals(id));
        
    }
}
