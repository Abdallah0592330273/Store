using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Store.DataAccess.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllByPropertyAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllByPropertyAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllByPropertyAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false,
            int? page = null,
            int? pageSize = null,
            params Expression<Func<T, object>>[] includeProperties);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<decimal> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        IQueryable<T> GetQueryable();
    }
}