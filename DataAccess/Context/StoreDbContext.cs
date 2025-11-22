using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context
{
    public class StoreDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Reviw> Reviws { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public StoreDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var baseDate = new DateTime(2025, 10, 21, 8, 50, 13, DateTimeKind.Utc);

            // Seed Categories (10)
            modelBuilder.Entity<Category>().HasData(
                new Category { categoryId = 1, categoryName = "Electronics", categoryDescription = "Devices and gadgets" },
                new Category { categoryId = 2, categoryName = "Books", categoryDescription = "Printed and digital books" },
                new Category { categoryId = 3, categoryName = "Clothing", categoryDescription = "Apparel for men and women" },
                new Category { categoryId = 4, categoryName = "Home", categoryDescription = "Home and kitchen" },
                new Category { categoryId = 5, categoryName = "Toys", categoryDescription = "Toys and games" },
                new Category { categoryId = 6, categoryName = "Sports", categoryDescription = "Sporting goods" },
                new Category { categoryId = 7, categoryName = "Beauty", categoryDescription = "Beauty and personal care" },
                new Category { categoryId = 8, categoryName = "Garden", categoryDescription = "Garden and outdoor" },
                new Category { categoryId = 9, categoryName = "Automotive", categoryDescription = "Automotive parts and accessories" },
                new Category { categoryId = 10, categoryName = "Health", categoryDescription = "Healthcare products" }
            );

            // Seed Users (10)
            modelBuilder.Entity<User>().HasData(
                new User { userId = 1, userName = "alice", email = "alice@example.com", password = "pass1", createdAt = baseDate.AddDays(-30), updatedAt = baseDate },
                new User { userId = 2, userName = "bob", email = "bob@example.com", password = "pass2", createdAt = baseDate.AddDays(-29), updatedAt = baseDate },
                new User { userId = 3, userName = "carol", email = "carol@example.com", password = "pass3", createdAt = baseDate.AddDays(-28), updatedAt = baseDate },
                new User { userId = 4, userName = "dave", email = "dave@example.com", password = "pass4", createdAt = baseDate.AddDays(-27), updatedAt = baseDate },
                new User { userId = 5, userName = "eve", email = "eve@example.com", password = "pass5", createdAt = baseDate.AddDays(-26), updatedAt = baseDate },
                new User { userId = 6, userName = "frank", email = "frank@example.com", password = "pass6", createdAt = baseDate.AddDays(-25), updatedAt = baseDate },
                new User { userId = 7, userName = "grace", email = "grace@example.com", password = "pass7", createdAt = baseDate.AddDays(-24), updatedAt = baseDate },
                new User { userId = 8, userName = "heidi", email = "heidi@example.com", password = "pass8", createdAt = baseDate.AddDays(-23), updatedAt = baseDate },
                new User { userId = 9, userName = "ivan", email = "ivan@example.com", password = "pass9", createdAt = baseDate.AddDays(-22), updatedAt = baseDate },
                new User { userId = 10, userName = "judy", email = "judy@example.com", password = "pass10", createdAt = baseDate.AddDays(-21), updatedAt = baseDate }
            );

            // Seed Products (10)
            modelBuilder.Entity<Product>().HasData(
                new Product { productId = 1, name = "Smartphone", description = "Latest smartphone", price = 699.99m, category = "Electronics", stockQuantity = 50, categoryId = 1 },
                new Product { productId = 2, name = "Laptop", description = "Lightweight laptop", price = 1199.50m, category = "Electronics", stockQuantity = 30, categoryId = 1 },
                new Product { productId = 3, name = "Novel", description = "Bestselling novel", price = 14.99m, category = "Books", stockQuantity = 200, categoryId = 2 },
                new Product { productId = 4, name = "T-Shirt", description = "Cotton t-shirt", price = 19.99m, category = "Clothing", stockQuantity = 150, categoryId = 3 },
                new Product { productId = 5, name = "Blender", description = "Kitchen blender", price = 49.99m, category = "Home", stockQuantity = 40, categoryId = 4 },
                new Product { productId = 6, name = "Board Game", description = "Family board game", price = 29.99m, category = "Toys", stockQuantity = 80, categoryId = 5 },
                new Product { productId = 7, name = "Running Shoes", description = "Comfort running shoes", price = 89.99m, category = "Sports", stockQuantity = 60, categoryId = 6 },
                new Product { productId = 8, name = "Skin Cream", description = "Moisturizing cream", price = 24.99m, category = "Beauty", stockQuantity = 120, categoryId = 7 },
                new Product { productId = 9, name = "Garden Hose", description = "Durable hose", price = 34.99m, category = "Garden", stockQuantity = 70, categoryId = 8 },
                new Product { productId = 10, name = "Vitamins", description = "Daily multivitamins", price = 12.99m, category = "Health", stockQuantity = 300, categoryId = 10 }
            );

            // Seed Orders (10)
            modelBuilder.Entity<Order>().HasData(
                new Order { orderId = 1, userId = 1, totalAmount = 150, status = "Completed", createdDate = baseDate.AddDays(-10) },
                new Order { orderId = 2, userId = 2, totalAmount = 299, status = "Processing", createdDate = baseDate.AddDays(-9) },
                new Order { orderId = 3, userId = 3, totalAmount = 45, status = "Shipped", createdDate = baseDate.AddDays(-8) },
                new Order { orderId = 4, userId = 4, totalAmount = 89, status = "Completed", createdDate = baseDate.AddDays(-7) },
                new Order { orderId = 5, userId = 5, totalAmount = 19, status = "Cancelled", createdDate = baseDate.AddDays(-6) },
                new Order { orderId = 6, userId = 6, totalAmount = 60, status = "Processing", createdDate = baseDate.AddDays(-5) },
                new Order { orderId = 7, userId = 7, totalAmount = 120, status = "Completed", createdDate = baseDate.AddDays(-4) },
                new Order { orderId = 8, userId = 8, totalAmount = 230, status = "Shipped", createdDate = baseDate.AddDays(-3) },
                new Order { orderId = 9, userId = 9, totalAmount = 15, status = "Completed", createdDate = baseDate.AddDays(-2) },
                new Order { orderId = 10, userId = 10, totalAmount = 75, status = "Processing", createdDate = baseDate.AddDays(-1) }
            );

            // Seed OrderItems (10)
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { orderItemId = 1, orderId = 1, productId = 1, quantity = 1, price = 699.99m },
                new OrderItem { orderItemId = 2, orderId = 2, productId = 2, quantity = 1, price = 1199.50m },
                new OrderItem { orderItemId = 3, orderId = 3, productId = 3, quantity = 2, price = 14.99m },
                new OrderItem { orderItemId = 4, orderId = 4, productId = 4, quantity = 3, price = 19.99m },
                new OrderItem { orderItemId = 5, orderId = 5, productId = 5, quantity = 1, price = 49.99m },
                new OrderItem { orderItemId = 6, orderId = 6, productId = 6, quantity = 1, price = 29.99m },
                new OrderItem { orderItemId = 7, orderId = 7, productId = 7, quantity = 1, price = 89.99m },
                new OrderItem { orderItemId = 8, orderId = 8, productId = 8, quantity = 2, price = 24.99m },
                new OrderItem { orderItemId = 9, orderId = 9, productId = 9, quantity = 1, price = 34.99m },
                new OrderItem { orderItemId = 10, orderId = 10, productId = 10, quantity = 5, price = 12.99m }
            );

            // Seed CartItems (10)
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem { cartItemId = 1, userId = 1, productId = 3, quantity = 1, dateAdded = baseDate.AddDays(-2) },
                new CartItem { cartItemId = 2, userId = 2, productId = 4, quantity = 2, dateAdded = baseDate.AddDays(-2) },
                new CartItem { cartItemId = 3, userId = 3, productId = 5, quantity = 1, dateAdded = baseDate.AddDays(-1) },
                new CartItem { cartItemId = 4, userId = 4, productId = 6, quantity = 3, dateAdded = baseDate.AddDays(-4) },
                new CartItem { cartItemId = 5, userId = 5, productId = 7, quantity = 1, dateAdded = baseDate.AddDays(-3) },
                new CartItem { cartItemId = 6, userId = 6, productId = 8, quantity = 2, dateAdded = baseDate.AddDays(-5) },
                new CartItem { cartItemId = 7, userId = 7, productId = 9, quantity = 1, dateAdded = baseDate.AddDays(-6) },
                new CartItem { cartItemId = 8, userId = 8, productId = 10, quantity = 1, dateAdded = baseDate.AddDays(-7) },
                new CartItem { cartItemId = 9, userId = 9, productId = 1, quantity = 1, dateAdded = baseDate.AddDays(-8) },
                new CartItem { cartItemId = 10, userId = 10, productId = 2, quantity = 1, dateAdded = baseDate.AddDays(-9) }
            );

            // Seed Reviews (Reviw) (10)
            modelBuilder.Entity<Reviw>().HasData(
                new Reviw { reviewId = 1, productId = 1, userId = 1, rating = 5, comment = "Excellent", CreatedAt = baseDate.AddDays(-20) },
                new Reviw { reviewId = 2, productId = 2, userId = 2, rating = 4, comment = "Very good", CreatedAt = baseDate.AddDays(-19) },
                new Reviw { reviewId = 3, productId = 3, userId = 3, rating = 3, comment = "Average", CreatedAt = baseDate.AddDays(-18) },
                new Reviw { reviewId = 4, productId = 4, userId = 4, rating = 5, comment = "Nice", CreatedAt = baseDate.AddDays(-17) },
                new Reviw { reviewId = 5, productId = 5, userId = 5, rating = 2, comment = "Not great", CreatedAt = baseDate.AddDays(-16) },
                new Reviw { reviewId = 6, productId = 6, userId = 6, rating = 4, comment = "Good", CreatedAt = baseDate.AddDays(-15) },
                new Reviw { reviewId = 7, productId = 7, userId = 7, rating = 5, comment = "Love it", CreatedAt = baseDate.AddDays(-14) },
                new Reviw { reviewId = 8, productId = 8, userId = 8, rating = 3, comment = "Okay", CreatedAt = baseDate.AddDays(-13) },
                new Reviw { reviewId = 9, productId = 9, userId = 9, rating = 4, comment = "Useful", CreatedAt = baseDate.AddDays(-12) },
                new Reviw { reviewId = 10, productId = 10, userId = 10, rating = 5, comment = "Recommended", CreatedAt = baseDate.AddDays(-11) }
            );

            // Seed Payments (10)
            modelBuilder.Entity<Payment>().HasData(
                new Payment { paymentId = 1, orderId = 1, method = "Card", amount = 150m, status = "Paid", createdAt = baseDate.AddDays(-10) },
                new Payment { paymentId = 2, orderId = 2, method = "Card", amount = 299m, status = "Pending", createdAt = baseDate.AddDays(-9) },
                new Payment { paymentId = 3, orderId = 3, method = "PayPal", amount = 45m, status = "Paid", createdAt = baseDate.AddDays(-8) },
                new Payment { paymentId = 4, orderId = 4, method = "Card", amount = 89m, status = "Paid", createdAt = baseDate.AddDays(-7) },
                new Payment { paymentId = 5, orderId = 5, method = "Card", amount = 19m, status = "Refunded", createdAt = baseDate.AddDays(-6) },
                new Payment { paymentId = 6, orderId = 6, method = "PayPal", amount = 60m, status = "Pending", createdAt = baseDate.AddDays(-5) },
                new Payment { paymentId = 7, orderId = 7, method = "Card", amount = 120m, status = "Paid", createdAt = baseDate.AddDays(-4) },
                new Payment { paymentId = 8, orderId = 8, method = "Card", amount = 230m, status = "Paid", createdAt = baseDate.AddDays(-3) },
                new Payment { paymentId = 9, orderId = 9, method = "PayPal", amount = 15m, status = "Paid", createdAt = baseDate.AddDays(-2) },
                new Payment { paymentId = 10, orderId = 10, method = "Card", amount = 75m, status = "Pending", createdAt = baseDate.AddDays(-1) }
            );
        }
    }
}