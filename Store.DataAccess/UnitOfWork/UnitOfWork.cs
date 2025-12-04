using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.DataAccess.Repositories;
using Store.DataAccess.Repositories.Interfaces;

namespace Store.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Repositories
        private IGenericRepository<Address> _addressRepo;
        private IGenericRepository<Cart> _cartRepo;
        private IGenericRepository<CartItem> _cartItemRepo;
        private IGenericRepository<Category> _categoryRepo;
        private IGenericRepository<Order> _orderRepo;
        private IGenericRepository<OrderItem> _orderItemRepo;
        private IGenericRepository<Payment> _paymentRepo;
        private IGenericRepository<Product> _productRepo;
        private IGenericRepository<Review> _reviewRepo;

        public UnitOfWork(
            StoreContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Repository Properties
        public IGenericRepository<Address> AddressRepo =>
            _addressRepo ??= new GenericRepository<Address>(_context);

        public IGenericRepository<Cart> CartRepo =>
            _cartRepo ??= new GenericRepository<Cart>(_context);

        public IGenericRepository<CartItem> CartItemRepo =>
            _cartItemRepo ??= new GenericRepository<CartItem>(_context);

        public IGenericRepository<Category> CategoryRepo =>
            _categoryRepo ??= new GenericRepository<Category>(_context);

        public IGenericRepository<Order> OrderRepo =>
            _orderRepo ??= new GenericRepository<Order>(_context);

        public IGenericRepository<OrderItem> OrderItemRepo =>
            _orderItemRepo ??= new GenericRepository<OrderItem>(_context);

        public IGenericRepository<Payment> PaymentRepo =>
            _paymentRepo ??= new GenericRepository<Payment>(_context);

        public IGenericRepository<Product> ProductRepo =>
            _productRepo ??= new GenericRepository<Product>(_context);

        public IGenericRepository<Review> ReviewRepo =>
            _reviewRepo ??= new GenericRepository<Review>(_context);

        // Identity Properties
        public UserManager<ApplicationUser> UserManager => _userManager;
        public RoleManager<IdentityRole> RoleManager => _roleManager;

        // Methods
        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public async Task<IDbContextTransaction> BeginTransactionAsync() =>
            await _context.Database.BeginTransactionAsync();

        // Dispose pattern
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}