using DataAccess.Context;
using Domain.Interfaces; // Contains IUnitOfWork and specific interfaces
using Domain.Interfaces.GenericInterfaces; // Contains IRepository<T>
using System;
using System.Threading.Tasks;
using Infastructure.Repositories; // Contains GenericRepository<T>
using Domain.Entities; // Contains User, Category, etc.

namespace Infastructure.UnitOfWork
{
    // UnitOfWork MUST implement IDisposable because the IUnitOfWork interface is expected to
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        // Private backing fields for lazy initialization
        private readonly StoreDbContext _context;
        private Domain.Interfaces.IUserRepository?  _userRepo;
        private ICategoryRepository? _categoryRepo;
        private ICartItemRepository? _cartItemRepo;
        private IOrderRepository? _orderRepo;
        private IOrderItemRepository? _orderItemRepo;
        private IPaymentRepository? _paymentRepo;
        private IProductRepository? _productRepo;
        private IReviewRepository? _reviewRepo;

        public UnitOfWork(StoreDbContext context)
        {
            _context = context;
        }

        // --- Public Repository Properties (Implementation of IUnitOfWork) ---

        // FIX: Instantiate GenericRepository<Entity> and cast it to the specific interface.
        // This is necessary because GenericRepository<User> only formally implements IRepository<User>, 
        // but it fulfills the contract of IUserRepository (which inherits from IRepository<User>).

        public Domain.Interfaces.IUserRepository Users =>
            _userRepo = (Domain.Interfaces.IUserRepository)new GenericRepository<User>(_context);

        public ICategoryRepository Categories =>
            _categoryRepo ??= (ICategoryRepository)new GenericRepository<Category>(_context);

        public ICartItemRepository CartItems =>
            _cartItemRepo ??= (ICartItemRepository)new GenericRepository<CartItem>(_context);

        public IOrderRepository Orders =>
            _orderRepo ??= (IOrderRepository)new GenericRepository<Order>(_context);

        public IOrderItemRepository OrderItems =>
            _orderItemRepo ??= (IOrderItemRepository)new GenericRepository<OrderItem>(_context);

        public IPaymentRepository Payments =>
            _paymentRepo ??= (IPaymentRepository)new GenericRepository<Payment>(_context);

        public IProductRepository Products =>
            _productRepo ??= (IProductRepository)new GenericRepository<Product>(_context);

        public IReviewRepository Reviews =>
            _reviewRepo ??= (IReviewRepository)new GenericRepository<Reviw>(_context);

        // --- Transaction Management ---

        // FIX: Implement the asynchronous commit (from IUnitOfWork)
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // --- IDisposable Implementation ---

        // FIX: Implement the Dispose method (from IDisposable/IUnitOfWork)
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}