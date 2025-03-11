using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Base
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(string id);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
            string includeProperties = null);

        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
            string includeProperties = null);

        Task AddAsync(T entity);

        Task Remove(string id);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);
    }
}
