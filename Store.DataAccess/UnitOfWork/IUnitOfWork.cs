using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Store.DataAccess.Entities;
using Store.DataAccess.Repositories.Interfaces;

namespace Store.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IGenericRepository<Address> AddressRepo { get; }
        IGenericRepository<Cart> CartRepo { get; }
        IGenericRepository<CartItem> CartItemRepo { get; }
        IGenericRepository<Category> CategoryRepo { get; }
        IGenericRepository<Order> OrderRepo { get; }
        IGenericRepository<OrderItem> OrderItemRepo { get; }
        IGenericRepository<Payment> PaymentRepo { get; }
        IGenericRepository<Product> ProductRepo { get; }
        IGenericRepository<Review> ReviewRepo { get; }

        // Identity
        UserManager<ApplicationUser> UserManager { get; }
        RoleManager<IdentityRole> RoleManager { get; }

        // Methods
        Task<int> SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}