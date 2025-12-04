using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Store.DataAccess.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string Method { get; set; } = string.Empty; // Credit Card, PayPal, Cash on Delivery

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed, Refunded

        [MaxLength(250)]
        public string? TransactionId { get; set; }

        [MaxLength(500)]
        public string? PaymentGatewayResponse { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = null;

        // Navigation Property
        [JsonIgnore]
        public Order? Order { get; set; }
    }
}