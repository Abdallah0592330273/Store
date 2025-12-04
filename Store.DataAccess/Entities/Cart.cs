using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Store.DataAccess.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = null;

        // Cart status (Active, Abandoned, Converted to Order)
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        // Navigation Properties
        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        [JsonIgnore]
        public ICollection<CartItem>? CartItems { get; set; } = new List<CartItem>();
    }
}