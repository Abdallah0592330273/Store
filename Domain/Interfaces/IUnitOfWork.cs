using Domain.Entities;
using Domain.Interfaces.GenericInterfaces;
using System; // Required for IDisposable
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    // MUST inherit from IDisposable for proper resource management
    public interface IUnitOfWork : IDisposable
    {
        // Exposed Repositories (one property for each entity)
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        ICartItemRepository CartItems { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IPaymentRepository Payments { get; }
        IProductRepository Products { get; }
        IReviewRepository Reviews { get; } // Assuming your entity is named Reviw, but the interface should be Review

        // Commit method (Asynchronous is standard practice)
        Task<int> CompleteAsync();
    }
}