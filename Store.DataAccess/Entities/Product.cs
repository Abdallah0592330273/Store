using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Store.DataAccess.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        [MaxLength(250)]
        public string? SKU { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        [Column(TypeName = "decimal(3, 2)")]
        [Range(0, 5)]
        public decimal? AverageRating { get; set; } = null;

        public int TotalReviews { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = null;

        // Navigation Properties
        [JsonIgnore]
        public Category? Category { get; set; }

        [JsonIgnore]
        public ICollection<CartItem>? CartItems { get; set; } = new List<CartItem>();

        [JsonIgnore]
        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();

        [JsonIgnore]
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }
}