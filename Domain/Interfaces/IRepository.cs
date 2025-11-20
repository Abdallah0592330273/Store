using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRepository <T> where T : class
    {
        Task? CreateAsync( T entity);
        Task? DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>>filter, bool isTracked =true );
        Task? GetAsync(Expression<Func<T, bool>> filter);
        Task ? UpdateAsync(T entity);
        void save();
    }
}
