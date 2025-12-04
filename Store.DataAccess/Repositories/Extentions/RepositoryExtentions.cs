using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Store.DataAccess.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<IEnumerable<T>> GetAllByPropertyAsync<T>(
            this IGenericRepository<T> repository,
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false,
            int? page = null,
            int? pageSize = null,
            params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            var query = repository.GetQueryable();

            // Apply includes
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            // Apply predicate
            query = query.Where(predicate);

            // Apply ordering
            if (orderBy != null)
            {
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            // Apply pagination
            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public static async Task<int> CountAsync<T>(
            this IGenericRepository<T> repository,
            Expression<Func<T, bool>> predicate) where T : class
        {
            var query = repository.GetQueryable();
            return await query.Where(predicate).CountAsync();
        }

        public static async Task<decimal> SumAsync<T>(
            this IGenericRepository<T> repository,
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, decimal>> selector) where T : class
        {
            var query = repository.GetQueryable();
            return await query.Where(predicate).SumAsync(selector);
        }
    }
}