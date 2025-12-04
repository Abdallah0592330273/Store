using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Store.DataAccess.Context;

public class StoreContext : IdentityDbContext<ApplicationUser>
{
    public StoreContext(DbContextOptions<StoreContext> options)
        : base(options)
    {
    }

    // DbSet definitions - USE PLURAL NAMES
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // MUST call base method first to set up Identity tables
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Carts)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Payment)
            .WithOne(p => p.Order)
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Decimal precision
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.Subtotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.TaxAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.ShippingCost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CartItem>()
            .Property(ci => ci.PriceSnapshot)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPriceSnapshot)
            .HasPrecision(18, 2);

        // Default values
        modelBuilder.Entity<Product>()
            .Property(p => p.IsActive)
            .HasDefaultValue(true);

        modelBuilder.Entity<Product>()
            .Property(p => p.IsFeatured)
            .HasDefaultValue(false);

        modelBuilder.Entity<Cart>()
            .Property(c => c.Status)
            .HasDefaultValue("Active");

        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasDefaultValue("Pending");

        modelBuilder.Entity<Payment>()
            .Property(p => p.Status)
            .HasDefaultValue("Pending");

        modelBuilder.Entity<Review>()
            .Property(r => r.Status)
            .HasDefaultValue("Pending");

        modelBuilder.Entity<Address>()
            .Property(a => a.IsShippingDefault)
            .HasDefaultValue(false);

        modelBuilder.Entity<Address>()
            .Property(a => a.IsBillingDefault)
            .HasDefaultValue(false);

        // ====================================================================
        // SEED DATA - FIXED VERSION (No duplicate entity seeding)
        // ====================================================================

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        const string password = "Password123!";
        var createdDate = DateTime.UtcNow.AddMonths(-1);

        // --- ROLE IDs ---
        var adminRoleId = Guid.NewGuid().ToString();
        var userRoleId = Guid.NewGuid().ToString();

        // --- USER IDs ---
        var adminId = Guid.NewGuid().ToString();  // Admin user
        var userId1 = Guid.NewGuid().ToString();  // Regular user 1
        var userId2 = Guid.NewGuid().ToString();  // Regular user 2

        // --- SEED ROLES ---
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = adminRoleId
            },
            new IdentityRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = userRoleId
            }
        );

        // --- SEED USERS (1 Admin, 2 Users) ---
        var users = new List<ApplicationUser>
        {
            // Admin User
            new ApplicationUser
            {
                Id = adminId,
                UserName = "admin@store.com",
                NormalizedUserName = "ADMIN@STORE.COM",
                Email = "admin@store.com",
                NormalizedEmail = "ADMIN@STORE.COM",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "+1234567890",
                CreatedDate = createdDate,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                LockoutEnabled = true
            },



            // Regular User 1
            new ApplicationUser
            {
                Id = userId1,
                UserName = "john.doe@example.com",
                NormalizedUserName = "JOHN.DOE@EXAMPLE.COM",
                Email = "john.doe@example.com",
                NormalizedEmail = "JOHN.DOE@EXAMPLE.COM",
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+1987654321",
                CreatedDate = createdDate.AddDays(1),
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                LockoutEnabled = true
            },
            // Regular User 2
            new ApplicationUser
            {
                Id = userId2,
                UserName = "jane.smith@example.com",
                NormalizedUserName = "JANE.SMITH@EXAMPLE.COM",
                Email = "jane.smith@example.com",
                NormalizedEmail = "JANE.SMITH@EXAMPLE.COM",
                EmailConfirmed = true,
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = "+1122334455",
                CreatedDate = createdDate.AddDays(2),
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                LockoutEnabled = true
            }
        };

        // Hash passwords
        foreach (var user in users)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, password);
        }
        modelBuilder.Entity<ApplicationUser>().HasData(users);

        // --- ASSIGN ROLES ---
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            // Admin user gets Admin role
            new IdentityUserRole<string> { UserId = adminId, RoleId = adminRoleId },

            // Regular users get User role
            new IdentityUserRole<string> { UserId = userId1, RoleId = userRoleId },
            new IdentityUserRole<string> { UserId = userId2, RoleId = userRoleId }
        );

        // --- SEED CATEGORIES ---
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices and gadgets",
                CreatedDate = createdDate
            },
            new Category
            {
                Id = 2,
                Name = "Clothing",
                Description = "Men's and women's clothing",
                CreatedDate = createdDate
            },
            new Category
            {
                Id = 3,
                Name = "Books",
                Description = "Books and publications",
                CreatedDate = createdDate
            }
        );

        // --- SEED PRODUCTS (with initial ratings) ---
        modelBuilder.Entity<Product>().HasData(
            // Electronics
            new Product
            {
                Id = 1,
                Name = "Wireless Headphones",
                Description = "Noise-cancelling wireless headphones",
                Price = 199.99m,
                StockQuantity = 50,
                SKU = "WH-001",
                ImageUrl = "/images/headphones.jpg",
                CategoryId = 1,
                IsActive = true,
                IsFeatured = true,
                AverageRating = null, // No reviews yet
                TotalReviews = 0,
                CreatedDate = createdDate
            },
            new Product
            {
                Id = 2,
                Name = "Smartphone",
                Description = "Latest smartphone with high-resolution camera",
                Price = 899.99m,
                StockQuantity = 30,
                SKU = "SP-001",
                ImageUrl = "/images/smartphone.jpg",
                CategoryId = 1,
                IsActive = true,
                IsFeatured = true,
                AverageRating = 5.0m, // Will have a 5-star review
                TotalReviews = 1,
                CreatedDate = createdDate
            },
            // Clothing
            new Product
            {
                Id = 3,
                Name = "Cotton T-Shirt",
                Description = "100% cotton t-shirt",
                Price = 24.99m,
                StockQuantity = 100,
                SKU = "TS-001",
                ImageUrl = "/images/tshirt.jpg",
                CategoryId = 2,
                IsActive = true,
                IsFeatured = false,
                AverageRating = null,
                TotalReviews = 0,
                CreatedDate = createdDate
            },
            new Product
            {
                Id = 4,
                Name = "Jeans",
                Description = "Denim jeans",
                Price = 59.99m,
                StockQuantity = 75,
                SKU = "JN-001",
                ImageUrl = "/images/jeans.jpg",
                CategoryId = 2,
                IsActive = true,
                IsFeatured = true,
                AverageRating = 4.0m, // Will have a 4-star review
                TotalReviews = 1,
                CreatedDate = createdDate
            },
            // Books
            new Product
            {
                Id = 5,
                Name = "Programming Book",
                Description = "Learn programming with this comprehensive guide",
                Price = 39.99m,
                StockQuantity = 25,
                SKU = "BK-001",
                ImageUrl = "/images/programming-book.jpg",
                CategoryId = 3,
                IsActive = true,
                IsFeatured = false,
                AverageRating = null,
                TotalReviews = 0,
                CreatedDate = createdDate
            }
        );

        // --- SEED ADDRESSES ---
        modelBuilder.Entity<Address>().HasData(
            // Addresses for John Doe
            new Address
            {
                Id = 1,
                UserId = userId1,
                Line1 = "123 Main Street",
                Line2 = "Apt 4B",
                City = "New York",
                StateProvince = "NY",
                ZipCode = "10001",
                Country = "USA",
                IsShippingDefault = true,
                IsBillingDefault = true,
                CreatedDate = createdDate
            },
            // Addresses for Jane Smith
            new Address
            {
                Id = 2,
                UserId = userId2,
                Line1 = "456 Oak Avenue",
                City = "Los Angeles",
                StateProvince = "CA",
                ZipCode = "90001",
                Country = "USA",
                IsShippingDefault = true,
                IsBillingDefault = true,
                CreatedDate = createdDate
            }
        );

        // --- SEED CARTS ---
        modelBuilder.Entity<Cart>().HasData(
            // Cart for John Doe
            new Cart
            {
                Id = 1,
                UserId = userId1,
                Status = "Active",
                CreatedDate = DateTime.UtcNow.AddDays(-3)
            },
            // Cart for Jane Smith
            new Cart
            {
                Id = 2,
                UserId = userId2,
                Status = "Active",
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            }
        );

        // --- SEED CART ITEMS ---
        modelBuilder.Entity<CartItem>().HasData(
            // John's cart items
            new CartItem
            {
                Id = 1,
                CartId = 1,
                ProductId = 1, // Wireless Headphones
                Quantity = 1,
                PriceSnapshot = 199.99m,
                DateAdded = DateTime.UtcNow.AddDays(-2)
            },
            new CartItem
            {
                Id = 2,
                CartId = 1,
                ProductId = 3, // Cotton T-Shirt
                Quantity = 2,
                PriceSnapshot = 24.99m,
                DateAdded = DateTime.UtcNow.AddDays(-1)
            },
            // Jane's cart items
            new CartItem
            {
                Id = 3,
                CartId = 2,
                ProductId = 5, // Programming Book
                Quantity = 1,
                PriceSnapshot = 39.99m,
                DateAdded = DateTime.UtcNow
            }
        );

        // --- SEED ORDERS (for John Doe) ---
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                UserId = userId1,
                OrderDate = DateTime.UtcNow.AddDays(-7),
                Subtotal = 299.97m,
                TaxAmount = 29.99m,
                ShippingCost = 9.99m,
                TotalAmount = 339.95m,
                Status = "Delivered",
                ShippingAddress = "123 Main Street, Apt 4B, New York, NY 10001, USA",
                ShippingMethod = "Standard",
                Notes = "Please leave at front door",
                ShippedDate = DateTime.UtcNow.AddDays(-5),
                DeliveredDate = DateTime.UtcNow.AddDays(-3),
                CreatedDate = DateTime.UtcNow.AddDays(-7)
            }
        );

        // --- SEED ORDER ITEMS ---
        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem
            {
                Id = 1,
                OrderId = 1,
                ProductId = 2, // Smartphone
                Quantity = 1,
                UnitPriceSnapshot = 899.99m,
                ProductNameSnapshot = "Smartphone",
                ProductDescriptionSnapshot = "Latest smartphone with high-resolution camera",
                CreatedDate = DateTime.UtcNow.AddDays(-7)
            },
            new OrderItem
            {
                Id = 2,
                OrderId = 1,
                ProductId = 4, // Jeans
                Quantity = 1,
                UnitPriceSnapshot = 59.99m,
                ProductNameSnapshot = "Jeans",
                ProductDescriptionSnapshot = "Denim jeans",
                CreatedDate = DateTime.UtcNow.AddDays(-7)
            }
        );

        // --- SEED PAYMENTS ---
        modelBuilder.Entity<Payment>().HasData(
            new Payment
            {
                Id = 1,
                OrderId = 1,
                Amount = 339.95m,
                Method = "Credit Card",
                Status = "Completed",
                TransactionId = "TXN_" + Guid.NewGuid().ToString("N").Substring(0, 12),
                PaymentDate = DateTime.UtcNow.AddDays(-7),
                CreatedDate = DateTime.UtcNow.AddDays(-7)
            }
        );

        // --- SEED REVIEWS ---
        modelBuilder.Entity<Review>().HasData(
            // John's review for Smartphone
            new Review
            {
                Id = 1,
                ProductId = 2,
                UserId = userId1,
                Rating = 5,
                Title = "Excellent Phone!",
                Body = "The camera quality is amazing and battery life lasts all day.",
                Status = "Approved",
                IsVerifiedPurchase = true,
                ReviewDate = DateTime.UtcNow.AddDays(-3),
                CreatedDate = DateTime.UtcNow.AddDays(-3)
            },
            // John's review for Jeans
            new Review
            {
                Id = 2,
                ProductId = 4,
                UserId = userId1,
                Rating = 4,
                Title = "Good quality",
                Body = "Fits well and comfortable, but a bit expensive.",
                Status = "Approved",
                IsVerifiedPurchase = true,
                ReviewDate = DateTime.UtcNow.AddDays(-2),
                CreatedDate = DateTime.UtcNow.AddDays(-2)
            }
        );

        // REMOVED THE DUPLICATE PRODUCT SEEDING HERE
        // The product ratings are already set in the initial Product seeding above
    }
}