using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Store.DataAccess.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(2000)]
        public string Body { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Approved"; // Approved, Pending, Rejected

        public bool IsVerifiedPurchase { get; set; } = false;

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = null;

        // Helpful votes
        public int HelpfulVotes { get; set; } = 0;
        public int UnhelpfulVotes { get; set; } = 0;

        // Navigation Properties
        [JsonIgnore]
        public Product? Product { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }
    }
}