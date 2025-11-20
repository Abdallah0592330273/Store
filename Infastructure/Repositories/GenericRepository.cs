using DataAccess.Context;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly StoreDbContext _db;
        private readonly DbSet<T> _dbset;

        public GenericRepository(StoreDbContext db, DbSet<T> dbset)
        {
            _db = db;
            _dbset = dbset;
        }

        public Task? CreateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task? DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter, bool isTracked = true)
        {
            throw new NotImplementedException();
        }

        public Task? GetAsync(Expression<Func<T, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public void save()
        {
            throw new NotImplementedException();
        }

        public Task? UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}