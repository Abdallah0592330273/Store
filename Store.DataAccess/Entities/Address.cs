using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Store.DataAccess.Entities
{
    public class Address
    {
        // Primary Key: Auto-generated (IDENTITY)
        public int Id { get; set; }

        // Foreign Key to the User (who owns this address)
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Data Properties
        [Required]
        [MaxLength(100)]
        public string Line1 { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Line2 { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string StateProvince { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        public bool IsShippingDefault { get; set; } = false;
        public bool IsBillingDefault { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = null;

        // Navigation Properties (Relationships)
        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}